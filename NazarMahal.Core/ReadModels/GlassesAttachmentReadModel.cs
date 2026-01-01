namespace NazarMahal.Core.ReadModels
{
    public class GlassesAttachmentReadModel
    {
        public int AttachmentId { get; set; }
        public int GlassesId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string StoragePath { get; set; }

    }
}
