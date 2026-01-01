namespace NazarMahal.Core.ReadModels
{
    public class GlassesReadModel
    {
        public int GlassesId { get; set; }
        public string GlassesName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string FrameType { get; set; }
        public string LensType { get; set; }
        public string Color { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        public bool IsActive { get; set; }

        public int AvailableQuantity { get; set; }
        public List<GlassesAttachmentReadModel> AttachmentReadModels { get; set; } = new List<GlassesAttachmentReadModel>();

    }
}
