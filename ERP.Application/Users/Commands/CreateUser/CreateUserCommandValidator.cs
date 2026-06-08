using FluentValidation;

namespace ERP.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.FullName)
            .MaximumLength(200);

        // Role is optional in the command; handler will default to "Employee".
        // If supplied, it must be a known role.
        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrWhiteSpace(role) ||
                          new[] { "Admin", "Manager", "Employee", "ReadOnly" }.Contains(role))
            .WithMessage("Invalid role. Allowed values: Admin, Manager, Employee, ReadOnly.");

        RuleFor(x => x.RequestedByUserId)
            .NotEmpty().WithMessage("RequestedByUserId is required.");

        RuleFor(x => x.UserRoles)
            .NotEmpty().WithMessage("UserRoles must not be null or empty.");
    }
}



/*
 
Consistency 
all your commands follow the same pipeline: Validator → Handler.
Omitting them for user commands would break that pattern and force you to remember why these two are different.


Early failure 
the validator catches missing fields (empty username, empty password) 
before the handler even touches Identity. It gives you a clean validation exception that the
UI already knows how to handle.


Separation of concerns 
the command layer doesn’t know or care that the implementation uses UserManager.
The validator is a pure guard on the command payload, independent of infrastructure. 

 */