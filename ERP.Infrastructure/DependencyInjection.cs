using ERP.Application.Common.Interfaces;
using ERP.Infrastructure.Persistence;
using ERP.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Infrastructure;

/// <summary>
/// Centralizes registration of all Infrastructure layer services.
/// Call this from Program.cs in both WebUI and WebAPI.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
        (
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        // --- Entity Framework Core ---
        // this is scoped (default behavior of AppDbContext)
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                });
        });

        // --- Repositories ---
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // --- Services ---
        services.AddScoped<ISaleNumberGenerator, SaleNumberGenerator>();
        services.AddScoped<ICustomerCodeGenerator, CustomerCodeGenerator>();

        // --- Unit of Work ---
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}