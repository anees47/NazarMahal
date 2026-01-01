using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Common;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Models;
using NazarMahal.Application.ResponseDto.AuthResponseDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Enums;

namespace NazarMahal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponseDto<RegisterResponseDto>>> Register([FromBody] RegisterModel model, [FromServices] NazarMahal.Core.Abstractions.IRequestContextAccessor requestContext)
        {
            var response = await _authService.Register(model, requestContext);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginModel model)
        {
            var response = await _authService.Login(model);
            return response.ToApiResponse();
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPost("RegisterAdmin")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> RegisterAdmin([FromBody] RegisterModel model)
        {
            var response = await _authService.RegisterAdmin(model);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ForgotPassword([FromForm] string email, [FromServices] NazarMahal.Core.Abstractions.IRequestContextAccessor requestContext)
        {
            var response = await _authService.ForgotPassword(email, requestContext);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ConfirmEmail(string userId, string token)
        {
            var response = await _authService.ConfirmEmail(userId, token);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpGet("ValidateResetToken")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ValidateResetToken(string userId, string token)
        {
            var response = await _authService.ValidateResetToken(userId, token);
            return response.ToApiResponse();
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ResetPassword([FromBody] NazarMahal.Application.Models.ResetPasswordRequest model)
        {
            var response = await _authService.ResetPassword(model);
            return response.ToApiResponse();
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var response = await _authService.ChangePassword(userId, model);
            return response.ToApiResponse();
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> RefreshToken()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var response = await _authService.RefreshToken(userId);
            return response.ToApiResponse();
        }

        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> GetCurrentUser()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var response = await _authService.GetCurrentUser(userId);
            return response.ToApiResponse();
        }

        [HttpPut("UpdateProfile")]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> UpdateProfile([FromBody] UpdateProfileRequest model)
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var response = await _authService.UpdateProfile(userId, model);
            return response.ToApiResponse();
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPut("UpdateUserRole/{userId}")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> UpdateUserRole(string userId, [FromBody] UpdateUserRoleRequest model)
        {
            var response = await _authService.UpdateUserRole(userId, model);
            return response.ToApiResponse();
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPut("ActivateUser/{userId}")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ActivateUser(string userId)
        {
            var response = await _authService.ActivateUser(userId);
            return response.ToApiResponse();
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPut("DeactivateUser/{userId}")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> DeactivateUser(string userId)
        {
            var response = await _authService.DeactivateUser(userId);
            return response.ToApiResponse();
        }

        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserListResponseDto>>>> GetAllUsers()
        {
            var response = await _authService.GetAllUsers();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAllCustomers")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserListResponseDto>>>> GetAllCustomers()
        {
            var response = await _authService.GetAllCustomers();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetUserById/{userId}")]
        public async Task<ActionResult<ApiResponseDto<UserProfileResponseDto>>> GetUserById(string userId)
        {
            var response = await _authService.GetUserById(userId);
            return response.ToApiResponse();
        }

        [HttpPost("Logout")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> Logout()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var response = await _authService.Logout(userId);
            return response.ToApiResponse();
        }
    }
}
