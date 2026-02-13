using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;

namespace Soat.Eleven.Kutcut.Users.Application.Validators;

public class UpdateUserInputValidator : AbstractValidator<UpdateUserInput>
{
    public UpdateUserInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do usuário é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório.")
            .EmailAddress()
            .WithMessage("O email deve ser válido.")
            .MaximumLength(100)
            .WithMessage("O email deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .MaximumLength(255)
            .WithMessage("A senha deve ter no máximo 255 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}
