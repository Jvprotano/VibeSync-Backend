using FluentValidation;
using VibeSync.Application.Requests;

namespace VibeSync.Application.Validators;

public class SpaceValidator : AbstractValidator<CreateSpaceRequest>
{
    public SpaceValidator()
    {
        RuleFor(space => space.UserEmail)
            .NotEmpty()
            .EmailAddress().WithMessage("O email do usuário é inválido.");

        RuleFor(space => space.Name)
            .NotEmpty()
            .MinimumLength(3).WithMessage("O nome do Space deve ter no mínimo 3 caracteres.");
    }
}
