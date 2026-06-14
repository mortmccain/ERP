using ERP.SharedKernel.Common;

namespace ERP.Application.Sales.Commands.MarkAsInvoiced;

public sealed class MarkAsInvoicedCommand
{
    public Guid SaleId { get; init; }
    public Guid MarkedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}