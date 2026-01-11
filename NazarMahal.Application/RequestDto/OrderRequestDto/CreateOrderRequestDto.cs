using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.RequestDto.OrderRequestDto
{
    public class CreateOrderRequestDto
    {
        public int UserId { get; set; }
        public required string UserEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public required string PaymentMethod { get; set; }

        [Required(ErrorMessage = "At least one order item is required")]
        public List<OrderItemRequestDto> OrderItems { get; set; } = [];
    }
}
