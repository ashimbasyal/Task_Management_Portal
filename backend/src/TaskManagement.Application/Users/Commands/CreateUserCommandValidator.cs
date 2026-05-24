using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Role).IsInEnum();

        // Department is mandatory for Officers
        RuleFor(x => x.DepartmentId)
            .NotNull()
            .WithMessage("Department is required for Officer role.")
            .When(x => x.Role == UserRole.Officer);
    }
}
