namespace NazarMahal.Application.DTOs.GlassesDto
{
    public class GlassesAttachmentDto
    {
        public int Id { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required string StoragePath { get; set; }
        public int ReferenceId { get; set; }
    }
}
