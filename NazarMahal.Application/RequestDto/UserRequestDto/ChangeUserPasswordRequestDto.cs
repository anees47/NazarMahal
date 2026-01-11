namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class ChangeUserPasswordRequestDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
