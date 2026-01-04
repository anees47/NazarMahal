using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.EntityConfiguration
{
    public class GlassesEntityConfiguration : IEntityTypeConfiguration<Glasses>
    {
        public void Configure(EntityTypeBuilder<Glasses> builder)
        {
            builder.ToTable("Glasses", "dbo");

            builder.HasKey(x => x.Id)
                .HasName("PK_Glasses")
                .IsClustered();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("Description")
                .HasColumnType("NVARCHAR(1000)");

            builder.Property(x => x.Price)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.Brand)
                .HasColumnName("Brand")
                .HasColumnType("NVARCHAR(100)");

            builder.Property(x => x.Model)
                .HasColumnName("Model")
                .HasColumnType("NVARCHAR(100)");

            builder.Property(x => x.FrameType)
                .HasColumnName("FrameType")
                .HasColumnType("NVARCHAR(100)");

            builder.Property(x => x.LensType)
                .HasColumnName("LensType")
                .HasColumnType("NVARCHAR(100)");

            builder.Property(x => x.Color)
                .HasColumnName("Color")
                .HasColumnType("NVARCHAR(100)");

            builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired();

            builder.HasOne<GlassesCategory>().WithMany().HasForeignKey(x => x.CategoryId).HasConstraintName("FK_Category_Id").IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<GlassesSubCategory>().WithMany().HasForeignKey(x => x.SubCategoryId).HasConstraintName("FK_SubCategory_Id").IsRequired().OnDelete(DeleteBehavior.Restrict);

        }
    }
}
