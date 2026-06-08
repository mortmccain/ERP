namespace ERP.Application.Customers.DTOs;

public sealed class CustomerListDto
{
    public Guid Id { get; init; }
    public string CustomerCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Email { get; init; }
    public bool IsActive { get; init; }
}