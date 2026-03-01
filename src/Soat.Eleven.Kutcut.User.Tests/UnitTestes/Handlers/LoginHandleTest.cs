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

public class LoginHandleTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IValidator<LoginInput>> _loginValidatorMock;
    private readonly LoginHandle _loginHandle;

    public LoginHandleTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _loginValidatorMock = new Mock<IValidator<LoginInput>>();

        _loginHandle = new LoginHandle(
            _userRepositoryMock.Object,
            _jwtTokenServiceMock.Object,
            _passwordServiceMock.Object,
            _loginValidatorMock.Object
        );
    }

    [Fact]
    public async Task LoginInput_RealizarLoginComCredenciaisValidas_RetornaLoginOutput()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var passwordHash = "hashed_password";
        var token = "jwt_token_example";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = input.Email,
            Password = passwordHash,
            Status = StatusUser.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.GetLoginAsync(input.Email, passwordHash))
            .ReturnsAsync(user);

        _jwtTokenServiceMock
            .Setup(j => j.GenerateToken(user))
            .Returns(token);

        var result = await _loginHandle.HandleAsync(input);

        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
        _passwordServiceMock.Verify(p => p.TransformToHash(input.Password), Times.Once);
        _userRepositoryMock.Verify(r => r.GetLoginAsync(input.Email, passwordHash), Times.Once);
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginInput_RealizarLoginComCredenciaisInvalidas_LancaException()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var passwordHash = "hashed_wrong_password";

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.GetLoginAsync(input.Email, passwordHash))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _loginHandle.HandleAsync(input));
        Assert.Contains("Invalid email or password", exception.Message);
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginInput_RealizarLoginComEmailInvalido_LancaException()
    {
        var input = new LoginInput
        {
            Email = "nonexistent@example.com",
            Password = "SecurePassword123"
        };

        var passwordHash = "hashed_password";

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.GetLoginAsync(input.Email, passwordHash))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _loginHandle.HandleAsync(input));
        Assert.Contains("Invalid email or password", exception.Message);
    }

    [Fact]
    public async Task LoginInput_RealizarLoginComDadosInvalidos_LancaValidationException()
    {
        var input = new LoginInput
        {
            Email = "",
            Password = ""
        };

        var validationErrors = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required"),
            new ValidationFailure("Password", "Password is required")
        };

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult(validationErrors));

        await Assert.ThrowsAsync<ValidationException>(() => _loginHandle.HandleAsync(input));

        _passwordServiceMock.Verify(p => p.TransformToHash(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(r => r.GetLoginAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginInput_RealizarLoginComEmailValidoESenhaInvalida_LancaException()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "WrongPassword123"
        };

        var passwordHash = "wrong_hashed_password";

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(input, default))
            .ReturnsAsync(new ValidationResult());

        _passwordServiceMock
            .Setup(p => p.TransformToHash(input.Password))
            .Returns(passwordHash);

        _userRepositoryMock
            .Setup(r => r.GetLoginAsync(input.Email, passwordHash))
            .ReturnsAsync((User?)null);

        var exception = await Assert.ThrowsAsync<Exception>(() => _loginHandle.HandleAsync(input));
        Assert.Contains("Invalid email or password", exception.Message);
        _passwordServiceMock.Verify(p => p.TransformToHash(input.Password), Times.Once);
        _userRepositoryMock.Verify(r => r.GetLoginAsync(input.Email, passwordHash), Times.Once);
        _jwtTokenServiceMock.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}
