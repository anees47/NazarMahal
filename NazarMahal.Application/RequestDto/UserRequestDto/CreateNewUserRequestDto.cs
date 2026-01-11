namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class CreateNewUserRequestDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Address { get; set; }
        public bool IsDisabled { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public required string Password { get; set; }
    }
}
