using FluentValidation.TestHelper;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Validators;

namespace Soat.Eleven.Kutcut.Users.Tests.UnitTests.Validators;

public class DeactiveUserInputValidatorTests
{
    private readonly DeactiveUserInputValidator _validator;

    public DeactiveUserInputValidatorTests()
    {
        _validator = new DeactiveUserInputValidator();
    }

    [Fact]
    public async Task DeactiveUserInput_ValidarComIdValido_NaoDeveRetornarErros()
    {
        var input = new DeactiveUserInput
        {
            Id = Guid.NewGuid()
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task DeactiveUserInput_ValidarComIdVazio_DeveRetornarErro()
    {
        var input = new DeactiveUserInput
        {
            Id = Guid.Empty
        };

        var result = await _validator.TestValidateAsync(input);

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("O ID do usuário é obrigatório.");
    }
}
