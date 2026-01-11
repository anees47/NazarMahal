using System.ComponentModel.DataAnnotations;
using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Application.RequestDto.AppointmentRequestDto
{
    public class AppointmentUpdateRequestDto
    {
        public int UserId { get; set; }
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public required AppointmentType AppointmentType { get; set; }
        public required string ReasonForVisit { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public required string PhoneNumber { get; set; }

        public string? AdditionalNotes { get; set; }
    }
}
