using System.ComponentModel.DataAnnotations;

namespace NazarMahal.Application.Models
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; }

        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
    }
}

