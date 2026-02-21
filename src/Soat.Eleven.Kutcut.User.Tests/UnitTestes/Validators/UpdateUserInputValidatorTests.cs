using FluentValidation.TestHelper;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Validators;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Validators;

public class UpdateUserInputValidatorTests
{
    private readonly UpdateUserInputValidator _validator;

    public UpdateUserInputValidatorTests()
    {
        _validator = new UpdateUserInputValidator();
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComDadosValidos_NaoDeveRetornarErros()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComDadosValidosSemSenha_NaoDeveRetornarErros()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = null!
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComIdVazio_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.Empty,
            Name = "Test User",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("O ID do usuário é obrigatório.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComNomeVazio_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("O nome é obrigatório.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComNomeNulo_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = null!,
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComNomeMaiorQue100Caracteres_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = new string('a', 101),
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("O nome deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComNomeCom100Caracteres_NaoDeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = new string('a', 100),
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComEmailVazio_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email é obrigatório.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComEmailInvalido_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "invalid-email",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ser válido.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComEmailSemArroba_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "testemail.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComEmailMaiorQue100Caracteres_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = new string('a', 90) + "@email.com.br",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComSenhaVazia_NaoDeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = ""
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComSenhaMenorQue6Caracteres_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "12345"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no mínimo 6 caracteres.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComSenhaCom6Caracteres_NaoDeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = "123456"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComSenhaMaiorQue255Caracteres_DeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = new string('a', 256)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no máximo 255 caracteres.");
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComSenhaCom255Caracteres_NaoDeveRetornarErro()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            Password = new string('a', 255)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task UpdateUserInput_ValidarComTodosCamposInvalidos_DeveRetornarMultiplosErros()
    {
        var input = new UpdateUserInput
        {
            Id = Guid.Empty,
            Name = "",
            Email = "invalid-email",
            Password = "123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
