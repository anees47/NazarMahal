using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.RequestDto.OrderRequestDto
{
    public class CreateOrderRequestDto
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; }
        
        [Required(ErrorMessage = "At least one order item is required")]
        public List<OrderItemRequestDto> OrderItems { get; set; } = new List<OrderItemRequestDto>();
    }
}
