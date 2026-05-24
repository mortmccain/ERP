using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ERP.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Build configuration from WebUI project (where appsettings.json lives)
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", "ERP.WebUI", "appsettings.json"),
                optional: false, reloadOnChange: true)
            .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "..", "ERP.WebUI", "appsettings.Development.json"),
                optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}