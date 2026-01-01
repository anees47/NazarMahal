namespace NazarMahal.Application.DTOs.AppointmentDto
{
    public class ScheduleAppointmentDto
    {
        public string FullName { get; set; }
        public string AppointmentType { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
