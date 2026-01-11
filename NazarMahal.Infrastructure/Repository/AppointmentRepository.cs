using Microsoft.EntityFrameworkCore;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.AppointmentRequestDto;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;
using NazarMahal.Infrastructure.Data;
using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Infrastructure.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AppointmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Appointment> CancelAppointment(int appointmentId)
        {
            var appointment = await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.AppointmentStatus == AppointmentStatus.Scheduled);

            if (appointment != null)
            {
                appointment.AppointmentStatus = AppointmentStatus.Cancelled;
                _ = _dbContext.Update(appointment);
                _ = await _dbContext.SaveChangesAsync();
            }

            return appointment;
        }

        public async Task<List<Appointment>> GetAppointmentsByUserId(int userId)
        {
            return await _dbContext.Appointments
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Appointment> UpdateAppointment(AppointmentUpdateRequestDto appointmentUpdateRequestDto)
        {
            var appointment = await _dbContext.Appointments.Where(a => a.Id == appointmentUpdateRequestDto.AppointmentId).FirstOrDefaultAsync();
            if (appointment != null)
            {
                appointment.AdditionalNotes = appointmentUpdateRequestDto.AdditionalNotes;
                appointment.AppointmentDate = appointmentUpdateRequestDto.AppointmentDate;
                appointment.AppointmentTime = appointmentUpdateRequestDto.AppointmentTime;
                appointment.AppointmentType = appointmentUpdateRequestDto.AppointmentType;
                appointment.ReasonForVisit = appointmentUpdateRequestDto.ReasonForVisit;


                _ = _dbContext.Appointments.Update(appointment);
                _ = await _dbContext.SaveChangesAsync();
            }

            return appointment;
        }
        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await _dbContext.Appointments.ToListAsync();
        }

        // New methods for additional endpoints
        public async Task<Appointment> GetAppointmentById(int appointmentId)
        {
            return await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);
        }

        public async Task<List<Appointment>> GetAppointmentsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            return await _dbContext.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByStatus(AppointmentEnums.AppointmentStatus status)
        {
            return await _dbContext.Appointments
                .Where(a => a.AppointmentStatus == status)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetTodaysAppointments()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _dbContext.Appointments
                .Where(a => a.AppointmentDate == today)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetUpcomingAppointments(int days)
        {
            var startDate = DateOnly.FromDateTime(DateTime.Today);
            var endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(days));

            return await _dbContext.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment> CompleteAppointment(int appointmentId, string? completionNotes = null)
        {
            var appointment = await _dbContext.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment != null)
            {
                appointment.AppointmentStatus = AppointmentStatus.Completed;
                if (!string.IsNullOrEmpty(completionNotes))
                {
                    appointment.AdditionalNotes = completionNotes;
                }

                _ = _dbContext.Appointments.Update(appointment);
                _ = await _dbContext.SaveChangesAsync();
            }

            return appointment;
        }

        public async Task<List<Appointment>> SearchAppointmentsByFullName(string fullName, DateOnly? date = null)
        {
            var query = _dbContext.Appointments.AsQueryable();

            if (!string.IsNullOrEmpty(fullName))
            {
                query = query.Where(a => a.FullName.Contains(fullName));
            }

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate == date.Value);
            }

            return await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByDateAndTime(DateOnly date, TimeSpan time)
        {
            return await _dbContext.Appointments
                .Where(a => a.AppointmentDate == date && a.AppointmentTime == time)
                .ToListAsync();
        }

        public async Task<Appointment> AddAppointmentAsync(Appointment appointment)
        {
            _ = _dbContext.Appointments.Add(appointment);
            _ = await _dbContext.SaveChangesAsync();
            return appointment;
        }

        public async Task<List<TimeSpan>> GetBookedTimeSlotsByDate(DateOnly date)
        {
            return await _dbContext.Appointments
                .Where(a => a.AppointmentDate == date)
                .Select(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<Appointment> UpdateAppointment(Appointment appointment)
        {
            _ = _dbContext.Appointments.Update(appointment);
            _ = await _dbContext.SaveChangesAsync();
            return appointment;
        }
    }
}
