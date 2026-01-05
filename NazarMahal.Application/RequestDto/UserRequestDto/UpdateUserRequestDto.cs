using Microsoft.AspNetCore.Http;
using NazarMahal.Core.Abstractions;

using System.Text.Json.Serialization;

namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class UpdateUserRequestDto
    {
        public int UserId { get; set; }

        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool IsDisabled { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
