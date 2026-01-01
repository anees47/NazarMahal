using NazarMahal.Application.DTOs.AppointmentDto;

namespace NazarMahal.Application.ResponseDto.AppointmentResponseDto
{
    public class AppointmentResponseDto
    {
        public string Message { get; set; }
        public AppointmentDto Appointment { get; set; }
    }
}
