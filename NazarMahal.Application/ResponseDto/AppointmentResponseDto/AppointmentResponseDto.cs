using NazarMahal.Application.DTOs.AppointmentDto;

namespace NazarMahal.Application.ResponseDto.AppointmentResponseDto
{
    public class AppointmentResponseDto
    {
        public required string Message { get; set; }
        public required AppointmentDto Appointment { get; set; }
    }
}
