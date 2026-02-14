using FluentValidation;
using FluentValidation.Results;
using Moq;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Handlers;
using Soat.Eleven.Kutcut.Users.Domain.Entities;
using Soat.Eleven.Kutcut.Users.Domain.Enums;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Repositories;
using Soat.Eleven.Kutcut.Users.Domain.Interfaces.Services;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Handlers;

public class UserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IValidator<CreateUserInput>> _createUserValidatorMock;
    private readonly Mock<IValidator<UpdateUserInput>> _updateUserValidatorMock;
    private readonly Mock<IValidator<DeactiveUserInput>> _deactiveUserValidatorMock;
    private readonly Mock<IValidator<GetUserInput>> _getUserValidatorMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly UserHandler _userHandler;

    public UserHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _createUserValidatorMock = new Mock<IValidator<CreateUserInput>>();
        _updateUserValidatorMock = new Mock<IValidator<UpdateUserInput>>();
        _deactiveUserValidatorMock = new Mock<IValidator<DeactiveUserInput>>();
        _getUserValidatorMock = new Mock<IValidator<GetUserInput>>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _passwordServiceMock = new Mock<IPasswordService>();

        _userHandler = new UserHandler(
            _userRepositoryMock.Object,
            _createUserValidatorMock.Object,
            _updateUserValidatorMock.Object,
            _deactiveUserValidatorMock.Object,
            _getUserValidatorMock.Object,
            _jwtTokenServiceMock.Object,
            _passwordServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateUserInput_CriarUsuarioComDadosValidos_RetornaCreateUserOutput()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var passwordHash = "hashed_password";

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _userHandler.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.Email, result.Email);
        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        _passwordServiceMock.Verify(p => p.TransformToHash(input.Password), Times.Once);
    }

    [Fact]
    public async Task CreateUserInput_CriarUsuarioComDadosInvalidos_LancaValidationException()
    {
        var input = new CreateUserInput
        {
            Name = "",
            Email = "invalid-email",
            Password = "123"
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Invalid email format")
        };

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        await Assert.ThrowsAsync<ValidationException>(() => _userHandler.HandleAsync(input));

        _userRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserInput_AtualizarUsuarioComDadosValidos_RetornaUpdateUserOutput()
    {
        var userId = Guid.NewGuid();
        var input = new UpdateUserInput
        {
            Id = userId,
            Name = "Updated User",
            Email = "updated@example.com",
            Password = "NewPassword123"
        };

        var existingUser = new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@example.com",
            Password = "old_hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        var passwordHash = "new_hashed_password";

        _jwtTokenServiceMock
            .Setup(j => j.GetUsuarioId())
            .Returns(userId);

        _updateUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _userHandler.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(input.Name, result.Name);
        Assert.Equal(input.Email, result.Email);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserInput_AtualizarUsuarioInexistente_LancaException()
    {
        var userId = Guid.NewGuid();
        var input = new UpdateUserInput
        {
            Id = userId,
            Name = "Updated User",
            Email = "updated@example.com",
            Password = "NewPassword123"
        };

        _jwtTokenServiceMock
            .Setup(j => j.GetUsuarioId())
            .Returns(userId);

        _updateUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _userHandler.HandleAsync(input));
        Assert.Contains("não encontrado", exception.Message);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUserInput_AtualizarUsuarioComDadosInvalidos_LancaValidationException()
    {
        var userId = Guid.NewGuid();
        var input = new UpdateUserInput
        {
            Id = userId,
            Name = "",
            Email = "invalid-email",
            Password = "123"
        };

        _jwtTokenServiceMock
            .Setup(j => j.GetUsuarioId())
            .Returns(userId);

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Invalid email format")
        };

        _updateUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        await Assert.ThrowsAsync<ValidationException>(() => _userHandler.HandleAsync(input));

        _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeactiveUserInput_DesativarUsuarioExistente_RetornaDeactiveUserOutput()
    {
        var userId = Guid.NewGuid();
        var input = new DeactiveUserInput
        {
            Id = userId
        };

        var existingUser = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _deactiveUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var result = await _userHandler.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal(StatusUser.Inactive, result.Active);
        Assert.Contains("desativado com sucesso", result.Message);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task DeactiveUserInput_DesativarUsuarioInexistente_LancaException()
    {
        var userId = Guid.NewGuid();
        var input = new DeactiveUserInput
        {
            Id = userId
        };

        _deactiveUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _userHandler.HandleAsync(input));
        Assert.Contains("não encontrado", exception.Message);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task DeactiveUserInput_DesativarUsuarioComDadosInvalidos_LancaValidationException()
    {
        var input = new DeactiveUserInput
        {
            Id = Guid.Empty
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Id is required")
        };

        _deactiveUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        await Assert.ThrowsAsync<ValidationException>(() => _userHandler.HandleAsync(input));

        _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task GetUserInput_BuscarUsuarioPorId_RetornaGetUserOutput()
    {
        var userId = Guid.NewGuid();
        var input = new GetUserInput
        {
            Id = userId,
            Email = string.Empty
        };

        var existingUser = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _getUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var result = await _userHandler.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(existingUser.Id, result.Id);
        Assert.Equal(existingUser.Name, result.Name);
        Assert.Equal(existingUser.Email, result.Email);
        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserInput_BuscarUsuarioPorEmail_RetornaGetUserOutput()
    {
        var email = "test@example.com";
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = email
        };

        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            Password = "hashed_password",
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _getUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(existingUser);

        var result = await _userHandler.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(existingUser.Id, result.Id);
        Assert.Equal(existingUser.Name, result.Name);
        Assert.Equal(existingUser.Email, result.Email);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task GetUserInput_BuscarUsuarioInexistente_LancaException()
    {
        var userId = Guid.NewGuid();
        var input = new GetUserInput
        {
            Id = userId,
            Email = string.Empty
        };

        _getUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _userHandler.HandleAsync(input));
        Assert.Contains("não encontrado", exception.Message);
    }

    [Fact]
    public async Task GetUserInput_BuscarUsuarioComDadosInvalidos_LancaValidationException()
    {
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = string.Empty
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Id or Email is required")
        };

        _getUserValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        await Assert.ThrowsAsync<ValidationException>(() => _userHandler.HandleAsync(input));

        _userRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }
}
