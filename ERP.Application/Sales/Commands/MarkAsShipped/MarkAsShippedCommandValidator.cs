using FluentValidation;

namespace ERP.Application.Sales.Commands.MarkAsShipped;

public sealed class MarkAsShippedCommandValidator : AbstractValidator<MarkAsShippedCommand>
{
    public MarkAsShippedCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(x => x.MarkedByUserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.UserRoles)
            .NotEmpty().WithMessage("User roles must be provided.")
            .Must(roles => roles.Contains("Admin") || roles.Contains("Manager"))
            .WithMessage("Only Admins and Managers can mark a sale as shipped.");
    }
}