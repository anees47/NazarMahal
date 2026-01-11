namespace NazarMahal.Core.ReadModels
{
    public class GlassesReadModel
    {
        public int GlassesId { get; set; }
        public required string GlassesName { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string FrameType { get; set; }
        public required string LensType { get; set; }
        public required string Color { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public required string SubCategoryName { get; set; }

        public bool IsActive { get; set; }

        public int AvailableQuantity { get; set; }
        public List<GlassesAttachmentReadModel> AttachmentReadModels { get; set; } = [];

    }
}
