using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;

namespace Soat.Eleven.Kutcut.Users.Application.Validators;

public class DeactiveUserInputValidator : AbstractValidator<DeactiveUserInput>
{
    public DeactiveUserInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");
    }
}
