using static NazarMahal.Core.Enums.AppointmentEnums;

namespace NazarMahal.Application.RequestDto.AppointmentRequestDto
{
    public class AppointmentCancelRequestDto
    {
        public int AppointmentId { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
    }
}
