namespace ERP.SharedKernel.DTOs;

public sealed class AddressDto
{
    public string Country { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;
    public string ExactAddress { get; init; } = string.Empty;
}