using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Application.RequestDto.AppointmentRequestDto
{
    /// <summary>
    /// Merged DTO for updating appointment details and status in a single PATCH request
    /// All fields are optional - only provided fields will be updated
    /// </summary>
    public class UpdateAppointmentMergedRequestDto
    {
        /// <summary>
        /// User ID (optional, for authorization context)
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// New appointment date (optional)
        /// </summary>
        public DateOnly? AppointmentDate { get; set; }

        /// <summary>
        /// New appointment time (optional)
        /// </summary>
        public TimeSpan? AppointmentTime { get; set; }

        /// <summary>
        /// Appointment type (optional)
        /// </summary>
        public AppointmentType? AppointmentType { get; set; }

        /// <summary>
        /// Reason for visit (optional)
        /// </summary>
        public string? ReasonForVisit { get; set; }

        /// <summary>
        /// Phone number (optional)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Appointment status (optional)
        /// </summary>
        public AppointmentStatus? AppointmentStatus { get; set; }

        /// <summary>
        /// Additional notes (optional)
        /// </summary>
        public string? AdditionalNotes { get; set; }
    }
}
