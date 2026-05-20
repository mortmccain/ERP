using ERP.Domain.Customers.Entities;
using ERP.Domain.Sales.Entities;
using ERP.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ERP.Infrastructure.Persistence;

/// <summary>
/// The main database context for the ERP system.
/// Inherits from IdentityDbContext to integrate ASP.NET Core Identity tables
/// alongside our domain tables.
/// </summary>
public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    // Domain entity DbSets
    /*
     
        // Old way - need a full property with getter/setter:
    public DbSet<Sale> Sales { get; set; }
    
    // OR even older way - need backing field:
    private DbSet<Sale> _sales;
    public DbSet<Sale> Sales 
    { 
        get => _sales; 
        set => _sales = value; 
    }
     
     */
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Customer> Customers => Set<Customer>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // passes the options (connection string to the base )
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // MUST call base first — this configures the Identity tables
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // this line gets all the configurations across the infrastructure so this method doesn't get as long as
        // our domain models are numbered.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}