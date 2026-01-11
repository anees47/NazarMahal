using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.EntityConfiguration
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            _ = builder.HasKey(oi => oi.OrderItemId);

            _ = builder.Property(oi => oi.Quantity)
                .IsRequired();

            _ = builder.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            _ = builder.Property(oi => oi.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            _ = builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasOne(oi => oi.Glasses)
                .WithMany()
                .HasForeignKey(oi => oi.GlassesId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

