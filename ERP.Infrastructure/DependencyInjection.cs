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
        services.AddDbContext<AppDbContext>
            (
            options =>
        {
            options.UseSqlServer
            (
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    // Enable retry on transient failures (important for 200+ users)
                    // Transient failures are temporary database hiccups. Not your fault.
                    // Not the database's fault. Just shit that happens.
                    sqlOptions.EnableRetryOnFailure
                    (
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    );
                }
            );
        });

        // --- Repositories ---
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // --- Services ---
        services.AddScoped<ISaleNumberGenerator, SaleNumberGenerator>();
        services.AddScoped<ICustomerCodeGenerator, CustomerCodeGenerator>();

        // --- Unit of Work ---
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}