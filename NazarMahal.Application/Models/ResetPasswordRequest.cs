namespace NazarMahal.Application.Models
{
    public class ResetPasswordRequest
    {
        public required string UserId { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}

