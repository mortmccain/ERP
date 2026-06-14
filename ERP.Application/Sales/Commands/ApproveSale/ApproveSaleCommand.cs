using ERP.SharedKernel.Common;

namespace ERP.Application.Sales.Commands.ApproveSale;

public sealed class ApproveSaleCommand
{
    public Guid SaleId { get; init; }
    public Guid ApprovedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}