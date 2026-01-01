namespace NazarMahal.Core.Enums
{
    public class AppointmentEnums
    {
        public enum AppointmentType
        {
            Consultation = 1,
            FollowUp = 2,
            Checkup = 4,


        }
        public enum AppointmentStatus
        {
            Scheduled = 1,
            Confirmed = 2,
            Completed = 4,
            Cancelled = 8,
        }
    }
}
