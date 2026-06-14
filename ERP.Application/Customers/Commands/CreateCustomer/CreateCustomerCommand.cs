namespace ERP.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Command to create a new Customer.
/// Returns the ID of the newly created Customer on success.
/// </summary>
public sealed class CreateCustomerCommand
{
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string Phone { get; init; } = string.Empty;
    public bool IsActive { get; init; } = true;
    public AddressInput BillingAddress { get; init; } = new();
    public AddressInput ShippingAddress { get; init; } = new();

    /// <summary>
    /// Nested DTO mirroring the Address value object fields.
    /// Defined here because it only exists in the context of this command.
    /// </summary>
    public sealed class AddressInput
    {
        public string Country { get; init; } = string.Empty;
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string ExactAddress { get; init; } = string.Empty;
    }
}