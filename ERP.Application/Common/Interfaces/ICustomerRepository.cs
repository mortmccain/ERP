using ERP.Domain.Customers.Entities;

namespace ERP.Application.Common.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Customer customer);
    // we don't delete customers we just deactivate them
}
