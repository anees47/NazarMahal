using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NazarMahal.Core.Entities;

namespace NazarMahal.Infrastructure.EntityConfiguration
{
    public class GlassesAttachmentEntityConfiguration : IEntityTypeConfiguration<GlassesAttachment>
    {
        public void Configure(EntityTypeBuilder<GlassesAttachment> builder)
        {
            _ = builder.ToTable("GlassesAttachment", "dbo");

            _ = builder.HasKey(x => x.Id)
                .HasName("PK_GlassesAttachmentId")
                .IsClustered();

            _ = builder.Property(x => x.GlassesId)
                .HasColumnName("GlassesId")
                .HasColumnType("INT")
                .IsRequired();

            _ = builder.Property(x => x.FileName)
                .HasColumnName("FileName")
                .HasColumnType("NVARCHAR(100)")
                .IsRequired();

            _ = builder.Property(x => x.FileType)
                            .HasColumnName("FileType")
                            .HasColumnType("NVARCHAR(10)")
                            .IsRequired();

            _ = builder.Property(x => x.StoragePath)
                .HasColumnName("StoragePath")
                .HasColumnType("NVARCHAR(500)")
                .IsRequired(false);

            _ = builder.Property(x => x.FileData)
                .HasColumnName("FileData")
                .HasColumnType("VARBINARY(MAX)")
                .IsRequired();

            _ = builder.HasOne<Glasses>()
                .WithMany(g => g.Attachments)
                .HasForeignKey(x => x.GlassesId)
                .HasConstraintName("FK_Glasses_Id")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
