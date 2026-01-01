using System.ComponentModel.DataAnnotations;
using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Application.RequestDto.AppointmentRequestDto
{
    public class ScheduleAppointmentRequestDto
    {
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }
        
        public string ReasonForVisit { get; set; }
        public string? AdditionalNotes { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
