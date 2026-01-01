using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;
using NazarMahal.Application.RequestDto.AppointmentRequestDto;

namespace NazarMahal.Application.Interfaces.IRepository
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsByDateAndTime(DateOnly date, TimeSpan time);
        Task<Appointment> AddAppointmentAsync(Appointment appointment);
        Task<List<TimeSpan>> GetBookedTimeSlotsByDate(DateOnly date);
        Task<Appointment> CancelAppointment(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByUserId(int userId);
        Task<Appointment> UpdateAppointment(AppointmentUpdateRequestDto appointmentUpdateRequestDto);
        Task<Appointment> UpdateAppointment(Appointment appointment);
        Task<List<Appointment>> GetAllAppointments();
        Task<Appointment> GetAppointmentById(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByDateRange(DateOnly startDate, DateOnly endDate);
        Task<List<Appointment>> GetAppointmentsByStatus(AppointmentEnums.AppointmentStatus status);
        Task<List<Appointment>> GetTodaysAppointments();
        Task<List<Appointment>> GetUpcomingAppointments(int days);
        Task<Appointment> CompleteAppointment(int appointmentId, string? completionNotes = null);
        Task<List<Appointment>> SearchAppointmentsByFullName(string fullName, DateOnly? date = null);
    }
}

