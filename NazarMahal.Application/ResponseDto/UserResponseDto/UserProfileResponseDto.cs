namespace NazarMahal.Application.ResponseDto.UserResponseDto
{
    public class UserProfileResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserType { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
