namespace NazarMahal.Application.DTOs.AppointmentDto
{
    public class ScheduleAppointmentDto
    {
        public required string FullName { get; set; }
        public required string AppointmentType { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Message { get; set; }
    }
}
