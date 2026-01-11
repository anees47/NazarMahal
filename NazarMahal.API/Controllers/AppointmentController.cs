using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Common;
using NazarMahal.Application.DTOs.AppointmentDto;
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
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> UpdateAppointment(int id, [FromBody] UpdateAppointmentMergedRequestDto request)
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

            // Get existing appointment to preserve FullName and Email if not provided
            var existingAppointmentResponse = await appointmentService.GetAppointmentById(id);
            if (!existingAppointmentResponse.IsSuccessful || existingAppointmentResponse.Payload == null)
            {
                return NotFound(new { Message = "Appointment not found" });
            }
            var existingAppointment = existingAppointmentResponse.Payload;

            var updateRequest = new AppointmentUpdateRequestDto
            {
                AppointmentId = id,
                UserId = request.UserId ?? 0,
                AppointmentDate = request.AppointmentDate ?? existingAppointment.AppointmentDate,
                AppointmentTime = request.AppointmentTime ?? existingAppointment.AppointmentTime,
                FullName = existingAppointment.FullName,
                Email = existingAppointment.Email,
                ReasonForVisit = request.ReasonForVisit ?? existingAppointment.ReasonForVisit,
                PhoneNumber = request.PhoneNumber ?? existingAppointment.PhoneNumber,
                AppointmentType = request.AppointmentType ?? existingAppointment.AppointmentType,
                AdditionalNotes = request.AdditionalNotes ?? existingAppointment.AdditionalNotes
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
