using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Application.DTOs.AppointmentDto
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
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
