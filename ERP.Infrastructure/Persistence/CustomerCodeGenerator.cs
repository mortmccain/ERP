using ERP.Application.Common.Interfaces;
using ERP.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence
{
    public sealed class CustomerCodeGenerator : ICustomerCodeGenerator
    {
        private readonly AppDbContext _dbContext;

        public CustomerCodeGenerator(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CustomerCode> NextAsync(string prefix, CancellationToken cancellationToken = default)
        {
            // Get the most recently created SaleNumber as a string
            var lastCustomerCodeString = await _dbContext.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAtUtc)
                .Select(c => c.CustomerCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastCustomerCodeString is null)
            {
                // First sale in the system
                return CustomerCode.First(prefix);
            }

            // We have a previous SaleNumber string like "SALE-2024-0042"
            // We need to parse it back into a SaleNumber object to call Next()
            // For now, extract the sequence number and increment
            var parts = lastCustomerCodeString.Value.Split('-');
            if (parts.Length == 3 && int.TryParse(parts[2], out int sequence))  // out says is TryParse succesful? if yes, put the value inside the sequence
            {
                var previous = CustomerCode.FromParts(parts[0], int.Parse(parts[1]), sequence);
                return CustomerCode.Next(previous, prefix);
            }

            // Fallback: if parsing fails, start fresh
            return CustomerCode.First(prefix);
        }
    }
}
