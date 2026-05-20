
namespace ERP.Application.Common.Interfaces;

/// <summary>
/// Coordinates persistence across multiple repositories.
/// Ensures all changes in a single use case are saved atomically.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
