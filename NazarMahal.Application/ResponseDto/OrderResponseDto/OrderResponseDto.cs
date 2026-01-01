using System.ComponentModel.DataAnnotations;
using NazarMahal.Application.DTOs.OrderDto;

namespace NazarMahal.Application.ResponseDto.OrderResponseDto
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        
        [MaxLength(100)]
        public string UserEmail { get; set; }
        
        [MaxLength(50)]
        public string OrderNumber { get; set; }
        
        [MaxLength(100)]
        public string FirstName { get; set; }
        
        [MaxLength(100)]
        public string LastName { get; set; }
        
        [MaxLength(50)]
        public string PaymentMethod { get; set; }
        
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}
