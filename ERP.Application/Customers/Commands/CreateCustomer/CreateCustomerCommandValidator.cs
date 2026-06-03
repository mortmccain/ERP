using FluentValidation;

namespace ERP.Application.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required.")
            .MaximumLength(300).WithMessage("Customer name cannot exceed 300 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(50).WithMessage("Phone number cannot exceed 50 characters.");

        // Email is optional — only validate format when it is actually provided
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("A valid email address is required.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.BillingAddress)
            .SetValidator(new AddressInputValidator("Billing"));

        RuleFor(x => x.ShippingAddress)
            .SetValidator(new AddressInputValidator("Shipping"));
    }
}

/// <summary>
/// Reusable validator for AddressInput.
/// Takes the address type label so error messages say "Billing address country is required"
/// instead of a generic message.
/// </summary>
internal sealed class AddressInputValidator : AbstractValidator<CreateCustomerCommand.AddressInput>
{
    public AddressInputValidator(string addressType)
    {
        RuleFor(x => x.Country)
            .NotEmpty().WithMessage($"{addressType} address country is required.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage($"{addressType} address street is required.")
            .MaximumLength(300).WithMessage($"{addressType} street cannot exceed 300 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage($"{addressType} address city is required.")
            .MaximumLength(100).WithMessage($"{addressType} city cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage($"{addressType} address postal code is required.")
            .MaximumLength(20).WithMessage($"{addressType} postal code cannot exceed 20 characters.");

        RuleFor(x => x.ExactAddress)
            .NotEmpty().WithMessage($"{addressType} exact address is required.")
            .MaximumLength(500).WithMessage($"{addressType} exact address cannot exceed 500 characters.");
    }
}