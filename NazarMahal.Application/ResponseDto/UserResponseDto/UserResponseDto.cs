namespace NazarMahal.Application.ResponseDto.UserResponseDto
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDisabled { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
