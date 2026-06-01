using FluentValidation;

namespace ERP.Application.Sales.Commands.SubmitSale;

public sealed class SubmitSaleCommandValidator : AbstractValidator<SubmitSaleCommand>
{
    public SubmitSaleCommandValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(x => x.SubmittedByUserId)
            .NotEmpty().WithMessage("Submitting user ID is required.");

        RuleFor(x => x.UserRoles)
            .NotEmpty().WithMessage("User roles must be provided.");
    }
}