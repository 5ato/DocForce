using DocForce.Application.Features.Auth.Commands;
using DocForce.Share;
using FluentValidation;

namespace DocForce.Application.Validators.Auth;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(c => c.Username)
            .NotEmpty()
            .MaximumLength(Constants.UsernameMaxLength);
        
        RuleFor(c => c.Password)
            .NotEmpty()
            .MaximumLength(Constants.PasswordMaxLength);

        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .Equal(c => c.Password);
    }
}
