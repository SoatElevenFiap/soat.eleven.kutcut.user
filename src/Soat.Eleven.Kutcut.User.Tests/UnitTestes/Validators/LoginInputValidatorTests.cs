using FluentValidation.TestHelper;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Validators;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Validators;

public class LoginInputValidatorTests
{
    private readonly LoginInputValidator _validator;

    public LoginInputValidatorTests()
    {
        _validator = new LoginInputValidator();
    }

    [Fact]
    public async Task LoginInput_ValidarComDadosValidos_NaoDeveRetornarErros()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailVazio_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email é obrigatório.");
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailNulo_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = null!,
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailInvalido_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "invalid-email",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ser válido.");
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailSemArroba_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "testemail.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailMaiorQue100Caracteres_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = new string('a', 90) + "@email.com.br",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("O email deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public async Task LoginInput_ValidarComEmailCom100Caracteres_NaoDeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = new string('a', 86) + "@example.com",
            Password = "SecurePassword123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaVazia_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = ""
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha é obrigatória.");
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaNula_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = null!
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaMenorQue6Caracteres_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "12345"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no mínimo 6 caracteres.");
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaCom6Caracteres_NaoDeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = "123456"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaMaiorQue255Caracteres_DeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = new string('a', 256)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("A senha deve ter no máximo 255 caracteres.");
    }

    [Fact]
    public async Task LoginInput_ValidarComSenhaCom255Caracteres_NaoDeveRetornarErro()
    {
        var input = new LoginInput
        {
            Email = "test@example.com",
            Password = new string('a', 255)
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public async Task LoginInput_ValidarComTodosCamposInvalidos_DeveRetornarMultiplosErros()
    {
        var input = new LoginInput
        {
            Email = "invalid-email",
            Password = "123"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
