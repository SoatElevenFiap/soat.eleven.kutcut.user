using FluentValidation.TestHelper;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Validators;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Validators;

public class GetUserInputValidatorTests
{
    private readonly GetUserInputValidator _validator;

    public GetUserInputValidatorTests()
    {
        _validator = new GetUserInputValidator();
    }

    [Fact]
    public async Task GetUserInput_ValidarComIdValido_NaoDeveRetornarErros()
    {
        var input = new GetUserInput
        {
            Id = Guid.NewGuid(),
            Email = null!
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetUserInput_ValidarComEmailValido_NaoDeveRetornarErros()
    {
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = "test@example.com"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetUserInput_ValidarComIdEEmailValidos_NaoDeveRetornarErros()
    {
        var input = new GetUserInput
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com"
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task GetUserInput_ValidarSemIdESemEmail_DeveRetornarErro()
    {
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = null!
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Informe o ID ou Email do usuário.");
    }

    [Fact]
    public async Task GetUserInput_ValidarComIdVazioEEmailVazio_DeveRetornarErro()
    {
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = ""
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Informe o ID ou Email do usuário.");
    }

    [Fact]
    public async Task GetUserInput_ValidarComIdVazioEEmailComEspacos_DeveRetornarErro()
    {
        var input = new GetUserInput
        {
            Id = Guid.Empty,
            Email = "   "
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Informe o ID ou Email do usuário.");
    }
}
