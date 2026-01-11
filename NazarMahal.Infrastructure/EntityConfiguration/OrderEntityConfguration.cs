using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;
using NazarMahal.Infrastructure.Data;

namespace NazarMahal.Infrastructure.EntityConfiguration;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        _ = builder.ToTable("Orders");

        _ = builder.HasKey(o => o.OrderId);

        _ = builder.Property(o => o.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        _ = builder.Property(o => o.OrderCreatedDate)
            .IsRequired();

        _ = builder.Property(o => o.OrderCreatedTime)
            .IsRequired();

        _ = builder.Property(o => o.FirstName)
           .HasMaxLength(100);

        _ = builder.Property(o => o.LastName)
            .HasMaxLength(100);

        _ = builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        _ = builder.Property(o => o.PhoneNumber)
            .HasMaxLength(20);

        _ = builder.Property(o => o.UserEmail)
            .HasMaxLength(100);

        _ = builder.Property(o => o.OrderStatus)
            .IsRequired();

        _ = builder.Property(o => o.PaymentMethod)
            .HasMaxLength(50);

        _ = builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
