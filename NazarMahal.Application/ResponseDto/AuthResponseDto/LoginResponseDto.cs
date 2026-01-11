namespace NazarMahal.Application.ResponseDto.AuthResponseDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public IEnumerable<ClaimDto> Claims { get; set; } = [];
        public string Message { get; set; } = string.Empty;
    }

    public class ClaimDto
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}



