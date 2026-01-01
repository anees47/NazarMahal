namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class ChangeUserPasswordRequestDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
