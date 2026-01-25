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

        [MaxLength(1000)]
        public string? StoragePath { get; set; }

        [Required]
        public byte[] FileData { get; set; } = [];

        public GlassesAttachment() { }

        public GlassesAttachment(int glassesId, string fileName, byte[] fileData, string fileType, string? storagePath = null)
        {
            GlassesId = glassesId;
            FileName = fileName;
            FileData = fileData;
            FileType = fileType;
            StoragePath = storagePath;
        }

        public static GlassesAttachment Create(int glassesId, string fileName, byte[] fileData, string fileType, string? storagePath = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("File name is required", nameof(fileName));
            }

            if (fileData == null || fileData.Length == 0)
            {
                throw new ArgumentException("File data is required", nameof(fileData));
            }

            return new GlassesAttachment(glassesId, fileName, fileData, fileType, storagePath);
        }
    }
}
