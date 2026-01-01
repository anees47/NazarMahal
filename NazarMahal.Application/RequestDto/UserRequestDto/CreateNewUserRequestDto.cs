namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class CreateNewUserRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsDisabled { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Password { get; internal set; }
    }
}
