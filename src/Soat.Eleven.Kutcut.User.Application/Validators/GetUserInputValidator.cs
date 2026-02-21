using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;

namespace Soat.Eleven.Kutcut.Users.Application.Validators;

public class GetUserInputValidator : AbstractValidator<GetUserInput>
{
    public GetUserInputValidator()
    {
        RuleFor(x => x)
            .Must(input => input.Id != Guid.Empty || !string.IsNullOrWhiteSpace(input.Email))
            .WithMessage("Informe o ID ou Email do usuário.");
    }
}
