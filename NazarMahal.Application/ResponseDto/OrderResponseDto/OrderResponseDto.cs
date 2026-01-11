using NazarMahal.Application.DTOs.OrderDto;
using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.ResponseDto.OrderResponseDto
{
    public class OrderResponseDto
    {
        public int OrderId { get; set; }
        public required string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public required string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }

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
