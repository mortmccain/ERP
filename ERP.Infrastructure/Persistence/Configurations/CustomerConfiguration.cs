using ERP.Domain.Customers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.OwnsOne(c => c.CustomerCode, code =>
        {
            code.Property(c => c.Value)
                .HasColumnName("CustomerCode")
                .HasMaxLength(50)
                .IsRequired();
            code.HasIndex(c => c.Value).IsUnique();
        });

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(200);
        builder.Property(c => c.Phone).HasMaxLength(50).IsRequired();

        // Addresses (Owned Value Objects)
        builder.OwnsOne(c => c.BillingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("BillingStreet").HasMaxLength(500);
            address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(200);
            address.Property(a => a.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(50);
        });

        builder.OwnsOne(c => c.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(500);
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(200);
            address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(50);
        });

        builder.Property(c => c.IsActive).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.LastOrderDate);

        builder.Ignore(c => c.DomainEvents);
    }
}