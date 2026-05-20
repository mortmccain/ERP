
namespace ERP.SharedKernel.DTOs;

public sealed class MoneyDto
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
}
