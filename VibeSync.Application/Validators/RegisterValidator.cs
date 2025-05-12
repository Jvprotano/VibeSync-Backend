using FluentValidation;
using VibeSync.Application.Requests;

namespace VibeSync.Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[a-zA-Z\d\W_]{8,}$")
            .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one number and one special character.");

        RuleFor(user => user.FullName)
        .NotEmpty().WithMessage("Full name is required.")
        .WithMessage("Full name is required.")
        .MinimumLength(3)
        .WithMessage("Full name must be at least 3 characters long.");
    }
}
