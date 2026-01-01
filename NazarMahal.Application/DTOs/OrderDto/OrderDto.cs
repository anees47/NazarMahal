using NazarMahal.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.DTOs.OrderDto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateOnly OrderCreatedDate { get; set; }
        public TimeOnly OrderCreatedTime { get; set; }
        
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
