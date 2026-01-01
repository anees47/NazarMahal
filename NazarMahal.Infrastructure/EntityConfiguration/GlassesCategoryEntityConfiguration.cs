using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.EntityConfiguration
{
    public class GlassesCategoryEntityConfiguration : IEntityTypeConfiguration<GlassesCategory>
    {
        public void Configure(EntityTypeBuilder<GlassesCategory> builder)
        {
            builder.ToTable("GlassesCategory", "dbo");

            builder.HasKey(x => x.Id)
                .HasName("PK_GlassesCategory")
                .IsClustered();

            builder.Property(x => x.Name)
                .HasColumnName("Name")
                .HasColumnType("NVARCHAR(500)")
                .IsRequired();


        }
    }
}
