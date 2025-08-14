using FluentValidation;

namespace ADR_T.ProductCatalog.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El username es requerido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("El password es requerido.");
    }
}
