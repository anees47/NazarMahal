using Microsoft.Extensions.Configuration;
using NazarMahal.Application.DTOs.AppointmentDto;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.Mappers;
using NazarMahal.Application.RequestDto.AppointmentRequestDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Common;
using NazarMahal.Core.Entities;
using NazarMahal.Core.Enums;

namespace NazarMahal.Application.Services
{
    public class AppointmentService(
        IEmailService emailService,
        IUserRepository userRepository,
        IAppointmentRepository appointmentRepository,
        INotificationService notificationService,
        IConfiguration configuration) : IAppointmentService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAppointmentRepository _appointmentRepository = appointmentRepository;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IEmailService _emailService = emailService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<ActionResponse<List<TimeSpan>>> GetAvailableTimeSlots(DateOnly date)
        {
            try
            {
                var bookedAppointments = await _appointmentRepository.GetBookedTimeSlotsByDate(date);

                var startHour = _configuration.GetValue<int>("BusinessHours:StartHour", 11);
                var endHour = _configuration.GetValue<int>("BusinessHours:EndHour", 19);
                var slotIntervalMinutes = _configuration.GetValue<int>("BusinessHours:SlotIntervalMinutes", 30);

                var businessHours = Enumerable.Range(startHour, endHour - startHour)
                    .SelectMany(h =>
                    {
                        var slots = new List<TimeSpan>();
                        for (int minutes = 0; minutes < 60; minutes += slotIntervalMinutes)
                        {
                            slots.Add(new TimeSpan(h, minutes, 0));
                        }
                        return slots;
                    });

                var availableSlots = businessHours.Except(bookedAppointments).ToList();
                return ActionResponse<List<TimeSpan>>.Ok(availableSlots);
            }
            catch (Exception ex)
            {
                return ActionResponse<List<TimeSpan>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> BookAppointment(ScheduleAppointmentRequestDto scheduleAppointmentRequestDto)
        {
            try
            {
                // Validate Pakistani phone number format
                if (string.IsNullOrEmpty(scheduleAppointmentRequestDto.PhoneNumber) ||
                    !System.Text.RegularExpressions.Regex.IsMatch(scheduleAppointmentRequestDto.PhoneNumber, @"^03\d{9}$"))
                {
                    return ActionResponse<AppointmentDto>.Fail("Phone number must be 11 digits.");
                }

                if (scheduleAppointmentRequestDto.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return ActionResponse<AppointmentDto>.Fail("Cannot book appointments in the past.");
                }

                if (scheduleAppointmentRequestDto.AppointmentDate == DateOnly.FromDateTime(DateTime.Today) &&
                    scheduleAppointmentRequestDto.AppointmentTime < TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan())
                {
                    return ActionResponse<AppointmentDto>.Fail("Cannot book appointments in the past.");
                }

                var existingAppointments = await _appointmentRepository.GetAppointmentsByDateAndTime(
                    scheduleAppointmentRequestDto.AppointmentDate,
                    scheduleAppointmentRequestDto.AppointmentTime);
                var existingAppointment = existingAppointments.Any();

                if (existingAppointment)
                {
                    return ActionResponse<AppointmentDto>.Fail("This slot is already reserved, please choose a different time.");
                }

                var appointment = new Appointment
                {
                    UserId = scheduleAppointmentRequestDto.UserId ?? 0,
                    FullName = scheduleAppointmentRequestDto.FullName,
                    Email = scheduleAppointmentRequestDto.Email,
                    PhoneNumber = scheduleAppointmentRequestDto.PhoneNumber,
                    ReasonForVisit = scheduleAppointmentRequestDto.ReasonForVisit,
                    AppointmentType = scheduleAppointmentRequestDto.AppointmentType,
                    AppointmentDate = scheduleAppointmentRequestDto.AppointmentDate,
                    AppointmentTime = scheduleAppointmentRequestDto.AppointmentTime,
                    AppointmentStatus = scheduleAppointmentRequestDto.AppointmentStatus,
                    AdditionalNotes = scheduleAppointmentRequestDto.AdditionalNotes,
                    DateCreated = PakistanTimeHelper.Now
                };

                if(scheduleAppointmentRequestDto.UserId.HasValue)
                {
                    var user = await _userRepository.GetUserByIdAsync(scheduleAppointmentRequestDto.UserId.Value);
                    if (user != null)
                    {
                        appointment.UserId = user.Id;
                        appointment.FullName = scheduleAppointmentRequestDto.FullName ?? user.FullName;
                        appointment.Email = scheduleAppointmentRequestDto.FullName ?? user.Email;
                    }
                }

               await _appointmentRepository.AddAppointmentAsync(appointment);
               
                await _notificationService.SendAppointmentConfirmationEmail(appointment);

                var appointmentDto = appointment.ToAppointmentDto();
                return ActionResponse<AppointmentDto>.Ok(appointmentDto, "Appointment booked successfully.");
            }
            catch (Exception ex)
            {
                return FailActionResponse<AppointmentDto>.Fail($"An error occurred while booking the appointment: {ex.Message}");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> CancelAppointment(AppointmentCancelRequestDto appointmentCancelRequestDto)
        {
            try
            {

                var appointment = await _appointmentRepository.CancelAppointment(appointmentCancelRequestDto.AppointmentId);
                if (appointment == null) return new NotFoundActionResponse<AppointmentDto>($"No Appointment is found for appointmentId {appointmentCancelRequestDto.AppointmentId}");

                var appointmentDto = appointment.ToAppointmentDto();
                return ActionResponse<AppointmentDto>.Ok(appointmentDto, "Appointment Cancelled Successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<AppointmentDto>.Fail("An error occurred while cancelling the appointment");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> UpdateAppointmentStatus(AppointmentCancelRequestDto appointmentCancelRequestDto)
        {
            try
            {
                if (appointmentCancelRequestDto == null)
                {
                    return new FailActionResponse<AppointmentDto>("Invalid request. Request body cannot be null.");
                }

                if (appointmentCancelRequestDto.AppointmentId <= 0)
                {
                    return new FailActionResponse<AppointmentDto>("Invalid appointment ID. Appointment ID must be greater than 0.");
                }

                var appointment = await _appointmentRepository.GetAppointmentById(appointmentCancelRequestDto.AppointmentId);

                if (appointment == null)
                {
                    return new NotFoundActionResponse<AppointmentDto>($"No appointment found with ID {appointmentCancelRequestDto.AppointmentId}");
                }

                appointment.AppointmentStatus = appointmentCancelRequestDto.AppointmentStatus;

                await _appointmentRepository.UpdateAppointment(appointment);

                if (appointmentCancelRequestDto.AppointmentStatus == AppointmentEnums.AppointmentStatus.Cancelled)
                {
                    await _notificationService.SendAppointmentCancellationEmail(appointment);
                }
                else if (appointmentCancelRequestDto.AppointmentStatus == AppointmentEnums.AppointmentStatus.Confirmed)
                {
                    await _notificationService.SendAppointmentConfirmationEmail(appointment);
                }
                else if (appointmentCancelRequestDto.AppointmentStatus == AppointmentEnums.AppointmentStatus.Completed)
                {
                    await _notificationService.SendAppointmentCompletionEmail(appointment);
                }

                var appointmentDto = appointment.ToAppointmentDto();
                return new OkActionResponse<AppointmentDto>(appointmentDto, $"Appointment status updated to {appointmentCancelRequestDto.AppointmentStatus} successfully");
            }
            catch (Exception ex)
            {
                return new FailActionResponse<AppointmentDto>($"An error occurred while updating appointment status: {ex.Message}");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> UpdateAppointment(AppointmentUpdateRequestDto appointmentUpdateRequestDto)
        {
            try
            {
                if (appointmentUpdateRequestDto == null)
                {
                    return new NotFoundActionResponse<AppointmentDto>("Appointment update request is invalid.");
                }
             
                if (appointmentUpdateRequestDto.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return new FailActionResponse<AppointmentDto>("Cannot update appointments to past dates.");
                }

                if (appointmentUpdateRequestDto.AppointmentDate == DateOnly.FromDateTime(DateTime.Today) &&
                    appointmentUpdateRequestDto.AppointmentTime < TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan())
                {
                    return new FailActionResponse<AppointmentDto>("Cannot update appointments to past times.");
                }

                var existingAppointments = await _appointmentRepository.GetAppointmentsByDateAndTime(
                    appointmentUpdateRequestDto.AppointmentDate,
                    appointmentUpdateRequestDto.AppointmentTime);
                var existingAppointment = existingAppointments.Any(a => a.Id != appointmentUpdateRequestDto.AppointmentId);

                if (existingAppointment)
                {
                    return new FailActionResponse<AppointmentDto>("An appointment already exists at this date and time. Please choose a different date or time");
                }

                var updateAppointment = await _appointmentRepository.UpdateAppointment(appointmentUpdateRequestDto);
                if (updateAppointment == null)
                {
                    return new NotFoundActionResponse<AppointmentDto>("Appointment couldn't be updated, please try again.");
                }

                await _notificationService.SendAppointmentUpdateConfirmationEmail(updateAppointment);
                var appointmentDto = updateAppointment.ToAppointmentDto();
                return ActionResponse<AppointmentDto>.Ok(appointmentDto, "Appointment Updated Successfully");

            }
            catch (Exception)
            {
                return FailActionResponse<AppointmentDto>.Fail("An error occurred while updating the appointment");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAllAppointments();
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>("No appointments found.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, "Appointments retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching the appointments");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentByUserId(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>("User not found.");
                }

                var appointments = await _appointmentRepository.GetAppointmentsByUserId(userId);
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>("No appointments found for the user.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos);
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching appointments by UserId.");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> GetAppointmentById(int appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetAppointmentById(appointmentId);
                if (appointment == null)
                {
                    return new NotFoundActionResponse<AppointmentDto>($"Appointment with ID {appointmentId} not found.");
                }

                var appointmentDto = appointment.ToAppointmentDto();
                return new OkActionResponse<AppointmentDto>(appointmentDto, "Appointment retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<AppointmentDto>.Fail("An error occurred while fetching the appointment.");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByDateRange(startDate, endDate);
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>($"No appointments found between {startDate:yyyy-MM-dd} and {endDate:yyyy-MM-dd}.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, "Appointments retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching appointments by date range.");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetAppointmentsByStatus(AppointmentEnums.AppointmentStatus status)
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByStatus(status);
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>($"No {status} appointments found.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, $"{status} appointments retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching appointments by status.");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetTodaysAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetTodaysAppointments();
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>("No appointments found for today.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, "Today's appointments retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching today's appointments.");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> GetUpcomingAppointments(int days)
        {
            try
            {
                if (days <= 0)
                {
                    return new FailActionResponse<IEnumerable<AppointmentDto>>("Days parameter must be greater than 0.");
                }

                var appointments = await _appointmentRepository.GetUpcomingAppointments(days);
                if (appointments == null || !appointments.Any())
                {
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>($"No upcoming appointments found for the next {days} days.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, $"Upcoming appointments for the next {days} days retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while fetching upcoming appointments.");
            }
        }

        public async Task<ActionResponse<AppointmentDto>> CompleteAppointment(int appointmentId, string completionNotes = null)
        {
            try
            {
                var appointment = await _appointmentRepository.CompleteAppointment(appointmentId, completionNotes);
                if (appointment == null)
                {
                    return new NotFoundActionResponse<AppointmentDto>($"Appointment with ID {appointmentId} not found.");
                }

                var appointmentDto = appointment.ToAppointmentDto();
                return new OkActionResponse<AppointmentDto>(appointmentDto, "Appointment completed successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<AppointmentDto>.Fail("An error occurred while completing the appointment.");
            }
        }

        public async Task<ActionResponse<IEnumerable<AppointmentDto>>> SearchAppointmentsByFullName(string fullName, DateOnly? date = null)
        {
            try
            {
                if (string.IsNullOrEmpty(fullName))
                {
                    return new FailActionResponse<IEnumerable<AppointmentDto>>("Full name is required for search.");
                }

                var appointments = await _appointmentRepository.SearchAppointmentsByFullName(fullName, date);
                if (appointments == null || !appointments.Any())
                {
                    var dateFilter = date.HasValue ? $" on {date.Value:yyyy-MM-dd}" : "";
                    return new NotFoundActionResponse<IEnumerable<AppointmentDto>>($"No appointments found for '{fullName}'{dateFilter}.");
                }

                var appointmentDtos = appointments.ToAppointmentDtoList();
                return new OkActionResponse<IEnumerable<AppointmentDto>>(appointmentDtos, "Search results retrieved successfully");
            }
            catch (Exception)
            {
                return FailActionResponse<IEnumerable<AppointmentDto>>.Fail("An error occurred while searching appointments.");
            }
        }
    }
}
