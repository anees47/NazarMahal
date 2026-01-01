using NazarMahal.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.Application.DTOs.AppointmentDto;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.AppointmentRequestDto;
using NazarMahal.Core.Enums;

namespace NazarMahal.API.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    [Authorize]
    public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
    {
        private readonly IAppointmentService _appointmentService = appointmentService;

        /// <summary>
        /// Get available time slots for a given date
        /// </summary>
        [AllowAnonymous]
        [HttpGet("availability")]
        public async Task<ActionResult<ApiResponseDto<List<TimeSpan>>>> GetAvailability([FromQuery] DateOnly date)
        {
            var response = await _appointmentService.GetAvailableTimeSlots(date);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Create a new appointment
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> CreateAppointment([FromBody] ScheduleAppointmentRequestDto request)
        {
            var response = await _appointmentService.BookAppointment(request);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> GetAppointment(int id)
        {
            var response = await _appointmentService.GetAppointmentById(id);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update appointment (details or status)
        /// Merged endpoint: replaces UpdateAppointmentDetails + UpdateAppointmentStatus
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> UpdateAppointment(
            int id,
            [FromBody] UpdateAppointmentMergedRequestDto request)
        {
            // If only status is provided, use status update
            if (request.AppointmentStatus.HasValue &&
                !request.AppointmentDate.HasValue &&
                !request.AppointmentTime.HasValue &&
                string.IsNullOrEmpty(request.ReasonForVisit) &&
                string.IsNullOrEmpty(request.PhoneNumber) &&
                request.AppointmentType == null)
            {
                var statusRequest = new AppointmentCancelRequestDto
                {
                    AppointmentId = id,
                    AppointmentStatus = request.AppointmentStatus.Value
                };
                var response = await _appointmentService.UpdateAppointmentStatus(statusRequest);
                return response.ToApiResponse();
            }

            // Otherwise, use full update
            var updateRequest = new AppointmentUpdateRequestDto
            {
                AppointmentId = id,
                UserId = request.UserId ?? 0,
                AppointmentDate = request.AppointmentDate ?? DateOnly.FromDateTime(DateTime.Today),
                AppointmentTime = request.AppointmentTime ?? TimeSpan.Zero,
                ReasonForVisit = request.ReasonForVisit ?? string.Empty,
                PhoneNumber = request.PhoneNumber,
                AppointmentType = request.AppointmentType ?? AppointmentEnums.AppointmentType.Consultation,
                AdditionalNotes = request.AdditionalNotes
            };
            var updateResponse = await _appointmentService.UpdateAppointment(updateRequest);
            return updateResponse.ToApiResponse();
        }

        /// <summary>
        /// Get appointments with filters
        /// Query params:
        /// - userId: filter by user
        /// - startDate: filter by date range start
        /// - endDate: filter by date range end
        /// - status: filter by appointment status
        /// - today: get only today's appointments
        /// - days: get upcoming appointments for N days
        /// - query: search by full name
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetAppointments(
            [FromQuery] int? userId = null,
            [FromQuery] DateOnly? startDate = null,
            [FromQuery] DateOnly? endDate = null,
            [FromQuery] AppointmentEnums.AppointmentStatus? status = null,
            [FromQuery] bool today = false,
            [FromQuery] int? days = null,
            [FromQuery] string? query = null)
        {
            // Search by full name
            if (!string.IsNullOrEmpty(query))
            {
                var response = await _appointmentService.SearchAppointmentsByFullName(query, startDate);
                return response.ToApiResponse();
            }

            // Get today's appointments
            if (today)
            {
                var response = await _appointmentService.GetTodaysAppointments();
                return response.ToApiResponse();
            }

            // Get upcoming appointments
            if (days.HasValue)
            {
                var response = await _appointmentService.GetUpcomingAppointments(days.Value);
                return response.ToApiResponse();
            }

            // Get appointments by date range
            if (startDate.HasValue && endDate.HasValue)
            {
                var response = await _appointmentService.GetAppointmentsByDateRange(startDate.Value, endDate.Value);
                return response.ToApiResponse();
            }

            // Get appointments by status
            if (status.HasValue)
            {
                var response = await _appointmentService.GetAppointmentsByStatus(status.Value);
                return response.ToApiResponse();
            }

            // Get appointments by user ID
            if (userId.HasValue)
            {
                var response = await _appointmentService.GetAppointmentByUserId(userId.Value);
                return response.ToApiResponse();
            }

            // Get all appointments
            var allResponse = await _appointmentService.GetAllAppointments();
            return allResponse.ToApiResponse();
        }
    }
}
