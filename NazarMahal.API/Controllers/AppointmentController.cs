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
        [AllowAnonymous]
        [HttpGet("availability")]
        public async Task<ActionResult<ApiResponseDto<List<TimeSpan>>>> GetAvailability([FromQuery] DateOnly date)
        {
            var response = await appointmentService.GetAvailableTimeSlots(date);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> CreateAppointment([FromBody] ScheduleAppointmentRequestDto request)
        {
            var response = await appointmentService.BookAppointment(request);
            return response.ToApiResponse();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> GetAppointment(int id)
        {
            var response = await appointmentService.GetAppointmentById(id);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update appointment (details or status)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> UpdateAppointment(
            int id,
            [FromBody] UpdateAppointmentMergedRequestDto request)
        {
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
                var response = await appointmentService.UpdateAppointmentStatus(statusRequest);
                return response.ToApiResponse();
            }

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
            var updateResponse = await appointmentService.UpdateAppointment(updateRequest);
            return updateResponse.ToApiResponse();
        }

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
            if (!string.IsNullOrEmpty(query))
            {
                var response = await appointmentService.SearchAppointmentsByFullName(query, startDate);
                return response.ToApiResponse();
            }

            if (today)
            {
                var response = await appointmentService.GetTodaysAppointments();
                return response.ToApiResponse();
            }

            if (days.HasValue)
            {
                var response = await appointmentService.GetUpcomingAppointments(days.Value);
                return response.ToApiResponse();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var response = await appointmentService.GetAppointmentsByDateRange(startDate.Value, endDate.Value);
                return response.ToApiResponse();
            }

            if (status.HasValue)
            {
                var response = await appointmentService.GetAppointmentsByStatus(status.Value);
                return response.ToApiResponse();
            }

            if (userId.HasValue)
            {
                var response = await appointmentService.GetAppointmentByUserId(userId.Value);
                return response.ToApiResponse();
            }

            var allResponse = await appointmentService.GetAllAppointments();
            return allResponse.ToApiResponse();
        }
    }
}
