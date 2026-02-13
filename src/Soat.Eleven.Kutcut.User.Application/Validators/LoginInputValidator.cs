using FluentValidation;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;

namespace Soat.Eleven.Kutcut.Users.Application.Validators;

public class LoginInputValidator : AbstractValidator<LoginInput>
{
    public LoginInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O email é obrigatório.")
            .EmailAddress()
            .WithMessage("O email deve ser válido.")
            .MaximumLength(100)
            .WithMessage("O email deve ter no máximo 100 caracteres.");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .MinimumLength(6)
            .WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .MaximumLength(255)
            .WithMessage("A senha deve ter no máximo 255 caracteres.");
    }
}
