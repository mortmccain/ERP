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
                .HasMaxLength(14)
                .IsRequired().ValueGeneratedNever();
            code.HasIndex(c => c.Value).IsUnique();
            code.Ignore(c => c.Prefix);
            code.Ignore(c => c.Year);
            code.Ignore(c => c.Sequence);
        });

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired().ValueGeneratedNever();
        builder.Property(c => c.Email).HasMaxLength(200).ValueGeneratedNever();
        builder.Property(c => c.Phone).HasMaxLength(50).IsRequired().ValueGeneratedNever();

        // Addresses (Owned Value Objects)
        builder.OwnsOne(c => c.BillingAddress, address =>
        {
            address.Property(a => a.Country).HasColumnName("BillingCountry").HasMaxLength(100).IsRequired();    // EF doesn't generate values for strings
            address.Property(a => a.Street).HasColumnName("BillingStreet").HasMaxLength(100).IsRequired().ValueGeneratedNever();
            address.Property(a => a.City).HasColumnName("BillingCity").HasMaxLength(200).IsRequired().ValueGeneratedNever();
            address.Property(a => a.PostalCode).HasColumnName("BillingPostalCode").HasMaxLength(10).IsRequired().ValueGeneratedNever();
            address.Property(a => a.ExactAddress).HasColumnName("ExactBillingAddress").IsRequired().ValueGeneratedNever();
        });

        builder.OwnsOne(c => c.ShippingAddress, address =>
        {
            address.Property(a => a.Country).HasColumnName("ShippingCountry").HasMaxLength(100).IsRequired();
            address.Property(a => a.Street).HasColumnName("ShippingStreet").HasMaxLength(500).IsRequired().ValueGeneratedNever();
            address.Property(a => a.City).HasColumnName("ShippingCity").HasMaxLength(200).IsRequired().ValueGeneratedNever();
            address.Property(a => a.PostalCode).HasColumnName("ShippingPostalCode").HasMaxLength(50).IsRequired().ValueGeneratedNever();
            address.Property(a => a.ExactAddress).HasColumnName("ExactShippingAddress").IsRequired().ValueGeneratedNever();

        });

        builder.Property(c => c.IsActive).IsRequired().ValueGeneratedNever();
        builder.Property(c => c.CreatedAtUtc).IsRequired().ValueGeneratedNever();
        builder.Property(c => c.LastOrderDateUtc).ValueGeneratedNever();

        builder.Ignore(c => c.DomainEvents);
    }
}