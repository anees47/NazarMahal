namespace NazarMahal.Application.DTOs.GlassesDto
{
    public class GlassesDto
    {
        public int Id { get; set; }
        public required string GlassesName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? FrameType { get; set; }
        public string? LensType { get; set; }
        public string? Color { get; set; }
        public int CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public int SubCategoryId { get; set; }
        public required string SubCategoryName { get; set; }
        public bool IsActive { get; set; }
        public int AvailableQuanity { get; set; }
        public required IEnumerable<GlassesAttachmentDto> Attachments { get; set; }
    }
}
