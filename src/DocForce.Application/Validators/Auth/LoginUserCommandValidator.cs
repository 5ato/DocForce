using DocForce.Application.Features.Auth.Commands;
using DocForce.Share;
using FluentValidation;

namespace DocForce.Application.Validators.Auth;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .MaximumLength(Constants.UsernameMaxLength);

        RuleFor(u => u.Password)
            .NotEmpty()
            .MaximumLength(Constants.PasswordMaxLength);
    }
}
