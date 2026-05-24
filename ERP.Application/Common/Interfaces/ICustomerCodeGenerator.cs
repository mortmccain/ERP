
using ERP.Domain.Customers.ValueObjects;

namespace ERP.Application.Common.Interfaces;
public interface ICustomerCodeGenerator
{
    Task<CustomerCode> NextAsync(string prefix, CancellationToken cancellationToken = default);
}
