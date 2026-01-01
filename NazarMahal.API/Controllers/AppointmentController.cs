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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [AllowAnonymous]
        [HttpGet("availableApptTime")]
        public async Task<ActionResult<ApiResponseDto<List<TimeSpan>>>> GetAvailableTimeSlots([FromQuery] DateOnly date)
        {
            var response = await _appointmentService.GetAvailableTimeSlots(date);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        //TODO: #1 validate if user has an appointment already booked. If not, book an appointment and check if user is a valid user with jwt token
        [HttpPost("scheduleAppointment")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> BookAppointment([FromBody] ScheduleAppointmentRequestDto scheduleAppointmentRequestDto)
        {
            var response = await _appointmentService.BookAppointment(scheduleAppointmentRequestDto);
            return response.ToApiResponse();
       }
        
        [HttpPost("UpdateAppointmentDetails")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> UpdateAppointment([FromBody] AppointmentUpdateRequestDto appointmentUpdateRequestDto)
        {
            var response = await _appointmentService.UpdateAppointment(appointmentUpdateRequestDto);
            return response.ToApiResponse();
        }

        [HttpPost("UpdateAppointmentStatus")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> UpdateAppointmentStatus([FromBody] AppointmentCancelRequestDto appointmentCancelRequestDto)
        {
            var response = await _appointmentService.UpdateAppointmentStatus(appointmentCancelRequestDto);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAllAppointments")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetAllAppointments()
        {
            var response = await _appointmentService.GetAllAppointments();
            return response.ToApiResponse();
        }                                                                              

        [HttpPost("GetAllAppointmentsByUserId")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetAllAppointmentsByUserId(int userId)
        {
            var response = await _appointmentService.GetAppointmentByUserId(userId);
            return response.ToApiResponse();
        }

        [HttpGet("GetAppointmentById/{appointmentId}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentDto>>> GetAppointmentById(int appointmentId)
        {
            var response = await _appointmentService.GetAppointmentById(appointmentId);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAppointmentsByDateRange")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetAppointmentsByDateRange([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            var response = await _appointmentService.GetAppointmentsByDateRange(startDate, endDate);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAppointmentsByStatus")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetAppointmentsByStatus([FromQuery] AppointmentEnums.AppointmentStatus status)
        {
            var response = await _appointmentService.GetAppointmentsByStatus(status);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetTodaysAppointments")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetTodaysAppointments()
        {
            var response = await _appointmentService.GetTodaysAppointments();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetNextSevenDaysAppointments")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> GetUpcomingAppointments([FromQuery] int days = 7)
        {
            var response = await _appointmentService.GetUpcomingAppointments(days);
            return response.ToApiResponse();
        }
        
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("SearchAppointments")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<AppointmentDto>>>> SearchAppointments([FromQuery] string query, [FromQuery] DateOnly? date = null)
        {
            var response = await _appointmentService.SearchAppointmentsByFullName(query, date);
            return response.ToApiResponse();
        }
    }
}
