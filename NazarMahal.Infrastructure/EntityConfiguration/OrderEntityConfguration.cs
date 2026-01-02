using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.Data.Configurations;

public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.OrderId);

        builder.Property(o => o.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.OrderStatus)
            .IsRequired();

        builder.Property(o => o.OrderCreatedDate)
            .IsRequired();

        builder.Property(o => o.OrderCreatedTime)
            .IsRequired();

        builder.Property(o => o.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(o => o.UserEmail)
            .HasMaxLength(100);

        builder.Property(o => o.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.FirstName)
            .HasMaxLength(100);

        builder.Property(o => o.LastName)
            .HasMaxLength(100);

        builder.Property(o => o.PaymentMethod)
            .HasMaxLength(50);

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
