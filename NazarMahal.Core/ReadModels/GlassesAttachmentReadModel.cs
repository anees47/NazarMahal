namespace NazarMahal.Core.ReadModels
{
    public class GlassesAttachmentReadModel
    {
        public int AttachmentId { get; set; }
        public int GlassesId { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required string StoragePath { get; set; }

    }
}
