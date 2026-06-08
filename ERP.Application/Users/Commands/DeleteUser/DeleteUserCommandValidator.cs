using FluentValidation;

namespace ERP.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

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