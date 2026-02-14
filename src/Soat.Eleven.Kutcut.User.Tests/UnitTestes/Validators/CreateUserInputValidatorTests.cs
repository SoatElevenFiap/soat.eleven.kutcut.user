using FluentValidation.TestHelper;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Validators;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Validators;

public class CreateUserInputValidatorTests
{
    private readonly CreateUserInputValidator _validator;

    public CreateUserInputValidatorTests()
    {
        _validator = new CreateUserInputValidator();
    }

    [Fact]
    public async Task CreateUserInput_ValidarComDadosValidos_NaoDeveRetornarErros()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateUserInput_ValidarComNomeVazio_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "",
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("O nome é obrigatório.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComNomeNulo_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = null!,
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CreateUserInput_ValidarComNomeMaiorQue100Caracteres_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = new string('a', 101),
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("O nome deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComNomeCom100Caracteres_NaoDeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = new string('a', 100),
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task CreateUserInput_ValidarComEmailVazio_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email é obrigatório.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComEmailInvalido_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "invalid-email",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ser válido.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComEmailSemArroba_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "testemail.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task CreateUserInput_ValidarComEmailMaiorQue100Caracteres_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = new string('a', 90) + "@email.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComSenhaVazia_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = ""
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha é obrigatória.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComSenhaMenorQue6Caracteres_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "12345"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no mínimo 6 caracteres.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComSenhaCom6Caracteres_NaoDeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "123456"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task CreateUserInput_ValidarComSenhaMaiorQue255Caracteres_DeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = new string('a', 256)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no máximo 255 caracteres.");
    }

    [Fact]
    public async Task CreateUserInput_ValidarComSenhaCom255Caracteres_NaoDeveRetornarErro()
    {
        var input = new CreateUserInput
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = new string('a', 255)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task CreateUserInput_ValidarComTodosCamposInvalidos_DeveRetornarMultiplosErros()
    {
        var input = new CreateUserInput
        {
            Name = "",
            Email = "invalid-email",
            Password = "123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
