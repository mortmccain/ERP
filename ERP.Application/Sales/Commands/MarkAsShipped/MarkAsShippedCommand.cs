using ERP.SharedKernel.Common;
using MediatR;

namespace ERP.Application.Sales.Commands.MarkAsShipped;

public sealed class MarkAsShippedCommand : IRequest<Result>
{
    public Guid SaleId { get; init; }
    public Guid MarkedByUserId { get; init; }
    public IReadOnlyList<string> UserRoles { get; init; } = Array.Empty<string>();
}