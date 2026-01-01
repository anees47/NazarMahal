using Microsoft.AspNetCore.Http;
using NazarMahal.Core.Abstractions;

namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    public class UpdateUserRequestDto
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsDisabled { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
