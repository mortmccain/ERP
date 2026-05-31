using ERP.Application.Common.Interfaces;
using ERP.Domain.Sales.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

/// <summary>
/// Generates sequential SaleNumbers by querying the last used number from the database.
/// </summary>
public sealed class SaleNumberGenerator : ISaleNumberGenerator
{
    private readonly AppDbContext _dbContext;

    public SaleNumberGenerator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SaleNumber> NextAsync(string prefix, CancellationToken cancellationToken = default)
    {
        // Get the most recently created SaleNumber as a string
        var lastSaleNumberString = await _dbContext.Sales
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAtUtc)
            .Select(s => s.SaleNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (lastSaleNumberString is null)
        {
            // First sale in the system
            return SaleNumber.First(prefix);
        }

        // We have a previous SaleNumber string like "SALE-2024-0042"
        // We need to parse it back into a SaleNumber object to call Next()
        // For now, extract the sequence number and increment
        var parts = lastSaleNumberString.Value.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out int sequence))  // out says is TryParse succesful? if yes, put the value inside the sequence
        {
            var previous = SaleNumber.FromParts(parts[0], int.Parse(parts[1]), sequence);
            return SaleNumber.Next(previous, prefix);
        }

        // Fallback: if parsing fails, start fresh
        return SaleNumber.First(prefix);
    }
}