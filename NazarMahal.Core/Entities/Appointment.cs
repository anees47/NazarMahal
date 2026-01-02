using System.ComponentModel.DataAnnotations;
using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Core.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        [RegularExpression(@"^03\d{9}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; } = null!;

        public string ReasonForVisit { get; set; } = null!;
        public string? AdditionalNotes { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
