using NazarMahal.Application.DTOs.AppointmentDto;
using NazarMahal.Core.Entities;

namespace NazarMahal.Application.Mappers
{
    /// <summary>
    /// Custom mapping extensions for Appointment-related DTOs
    /// </summary>
    public static class AppointmentMappingExtensions
    {
        /// <summary>
        /// Map Appointment to AppointmentDto
        /// </summary>
        public static AppointmentDto ToAppointmentDto(this Appointment appointment)
        {
            if (appointment == null) return null;

            return new AppointmentDto
            {
                Id = appointment.Id,
                UserId = appointment.UserId,
                FullName = appointment.FullName,
                Email = appointment.Email,
                PhoneNumber = appointment.PhoneNumber,
                AppointmentType = appointment.AppointmentType,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                AppointmentStatus = appointment.AppointmentStatus,
                ReasonForVisit = appointment.ReasonForVisit,
                AdditionalNotes = appointment.AdditionalNotes,
                DateCreated = appointment.DateCreated
            };
        }

        /// <summary>
        /// Map IEnumerable of Appointment to List of AppointmentDto
        /// </summary>
        public static List<AppointmentDto> ToAppointmentDtoList(this IEnumerable<Appointment> appointments)
        {
            if (appointments == null) return new List<AppointmentDto>();
            return appointments.Select(a => a.ToAppointmentDto()).ToList();
        }
    }
}
