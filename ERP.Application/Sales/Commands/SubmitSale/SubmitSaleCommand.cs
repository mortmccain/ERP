using ERP.SharedKernel.Common;

namespace ERP.Application.Sales.Commands.SubmitSale;

public sealed class SubmitSaleCommand : IRequest<Result>
{
    public Guid SaleId { get; init; }
    public Guid SubmittedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}