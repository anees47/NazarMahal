namespace NazarMahal.Application.ResponseDto.UserResponseDto
{
    public class UserProfileResponseDto
    {
        public required string UserId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public required string UserType { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
