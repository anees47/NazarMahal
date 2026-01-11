using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.EntityConfiguration
{
    public class GlassesSubCategoryEntityConfiguration : IEntityTypeConfiguration<GlassesSubCategory>
    {
        public void Configure(EntityTypeBuilder<GlassesSubCategory> builder)
        {
            _ = builder.ToTable("GlassesSubCategory", "dbo");

            _ = builder.HasKey(x => x.Id)
                .HasName("PK_GlassesSubcategory")
                .IsClustered();

            _ = builder.Property(x => x.Name)
                           .HasColumnName("Name")
                           .HasColumnType("NVARCHAR(500)")
                           .IsRequired();

            _ = builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .HasColumnType("bit")
            .IsRequired();

            _ = builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .HasConstraintName("FK_CategoryId")
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
