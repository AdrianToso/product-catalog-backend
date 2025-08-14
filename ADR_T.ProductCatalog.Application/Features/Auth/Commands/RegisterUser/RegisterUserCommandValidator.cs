using FluentValidation;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El username es requerido.")
            .MaximumLength(50).WithMessage("El username no debe exceder 50 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("El password es requerido.")
            .MinimumLength(6).WithMessage("El password debe tener al menos 6 caracteres.");
    }
}
