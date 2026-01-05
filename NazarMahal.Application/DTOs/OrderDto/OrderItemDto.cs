using NazarMahal.Application.DTOs.GlassesDto;

namespace NazarMahal.Application.DTOs.OrderDto
{
    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int GlassesId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string? GlassesName { get; set; }
        
        // Full glasses details
        public GlassesDetailsDto? GlassesDetails { get; set; }
    }

    /// <summary>
    /// Simplified glasses info for order items
    /// </summary>
    public class GlassesDetailsDto
    {
        public int GlassesId { get; set; }
        public string GlassesName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? FrameType { get; set; }
        public string? LensType { get; set; }
        public string? Color { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public List<GlassesAttachmentDto>? Attachments { get; set; }
    }
}

