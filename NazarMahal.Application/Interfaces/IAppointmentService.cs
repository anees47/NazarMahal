using NazarMahal.Application.DTOs.AppointmentDto;
using NazarMahal.Application.RequestDto.AppointmentRequestDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Enums;

namespace NazarMahal.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<ActionResponse<List<TimeSpan>>> GetAvailableTimeSlots(DateOnly date);
        Task<ActionResponse<AppointmentDto>> BookAppointment(ScheduleAppointmentRequestDto scheduleAppointmentRequestDto);
        Task<ActionResponse<AppointmentDto>> CancelAppointment(AppointmentCancelRequestDto appointmentCancelRequestDto);
        Task<ActionResponse<AppointmentDto>> UpdateAppointmentStatus(AppointmentCancelRequestDto appointmentCancelRequestDto);
        Task<ActionResponse<AppointmentDto>> UpdateAppointment(AppointmentUpdateRequestDto appointmentUpdateRequestDto);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentByUserId(int userId);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAllAppointments();
        Task<ActionResponse<AppointmentDto>> GetAppointmentById(int appointmentId);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByDateRange(DateOnly startDate, DateOnly endDate);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByStatus(AppointmentEnums.AppointmentStatus status);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetTodaysAppointments();
        Task<ActionResponse<IEnumerable<AppointmentDto>>> GetUpcomingAppointments(int days);
        Task<ActionResponse<AppointmentDto>> CompleteAppointment(int appointmentId, string? completionNotes = null);
        Task<ActionResponse<IEnumerable<AppointmentDto>>> SearchAppointmentsByFullName(string fullName, DateOnly? date = null);
    }
}
