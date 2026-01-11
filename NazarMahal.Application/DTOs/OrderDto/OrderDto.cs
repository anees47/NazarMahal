using NazarMahal.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.DTOs.OrderDto
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateOnly OrderCreatedDate { get; set; }
        public TimeOnly OrderCreatedTime { get; set; }

        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [MaxLength(100)]
        public required string UserEmail { get; set; }

        [MaxLength(50)]
        public required string OrderNumber { get; set; }

        [MaxLength(100)]
        public required string FirstName { get; set; }

        [MaxLength(100)]
        public required string LastName { get; set; }

        [MaxLength(50)]
        public required string PaymentMethod { get; set; }

        public List<OrderItemDto> OrderItems { get; set; } = [];
    }
}
