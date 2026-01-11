using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Core.Entities
{
    public class GlassesAttachment
    {
        public int Id { get; set; }

        [Required]
        public int GlassesId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string StoragePath { get; set; } = string.Empty;

        public GlassesAttachment() { }

        public GlassesAttachment(int glassesId, string fileName, string storagePath, string fileType)
        {
            GlassesId = glassesId;
            FileName = fileName;
            StoragePath = storagePath;
            FileType = fileType;
        }

        public static GlassesAttachment Create(int glassesId, string fileName, string fullFileRelativePath, string fileType)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required", nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(fullFileRelativePath))
            {
                throw new ArgumentException("Storage path is required", nameof(fullFileRelativePath));
            }

            return new GlassesAttachment(glassesId, fileName, fullFileRelativePath, fileType);
        }
    }
}
