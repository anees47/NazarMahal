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
    }
}

