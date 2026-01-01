using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.Models
{
    public class RegisterModel
    {
        public required string Name { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
