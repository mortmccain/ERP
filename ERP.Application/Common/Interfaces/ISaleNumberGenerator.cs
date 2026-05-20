using ERP.Domain.Sales.ValueObjects;

namespace ERP.Application.Common.Interfaces;

public interface ISaleNumberGenerator
{
    Task<SaleNumber> NextAsync(string prefix, CancellationToken cancellationToken = default);
}