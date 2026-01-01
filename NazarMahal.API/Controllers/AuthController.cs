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
    [Route("api/auth")]
    [ApiController]
    [Authorize]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Register a new user account
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDto<RegisterResponseDto>>> Register([FromBody] RegisterModel model, [FromServices] NazarMahal.Core.Abstractions.IRequestContextAccessor requestContext)
        {
            var response = await _authService.Register(model, requestContext);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto<LoginResponseDto>>> Login([FromBody] LoginModel model)
        {
            var response = await _authService.Login(model);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Logout the current user
        /// </summary>
        [HttpPost("logout")]
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

        /// <summary>
        /// Refresh authentication token
        /// </summary>
        [HttpPost("refresh-token")]
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

        /// <summary>
        /// Request password reset (forgot password)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ForgotPassword([FromForm] string email, [FromServices] NazarMahal.Core.Abstractions.IRequestContextAccessor requestContext)
        {
            var response = await _authService.ForgotPassword(email, requestContext);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var response = await _authService.ResetPassword(model);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Confirm email with token
        /// </summary>
        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ConfirmEmail(string userId, string token)
        {
            var response = await _authService.ConfirmEmail(userId, token);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Validate password reset token
        /// </summary>
        [AllowAnonymous]
        [HttpGet("validate-reset-token")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ValidateResetToken(string userId, string token)
        {
            var response = await _authService.ValidateResetToken(userId, token);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Change password for current user
        /// </summary>
        [HttpPost("change-password")]
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

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
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

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("profile")]
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

        /// <summary>
        /// Register a new admin user (SuperAdmin only)
        /// </summary>
        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPost("admin")]
        public async Task<ActionResult<ApiResponseDto<TokenResponseDto>>> RegisterAdmin([FromBody] RegisterModel model)
        {
            var response = await _authService.RegisterAdmin(model);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update user role (SuperAdmin only)
        /// </summary>
        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPatch("users/{userId}/role")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> UpdateUserRole(
            string userId,
            [FromBody] UpdateUserRoleRequest model)
        {
            var response = await _authService.UpdateUserRole(userId, model);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Activate user account (SuperAdmin only)
        /// </summary>
        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPatch("users/{userId}/activate")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> ActivateUser(string userId)
        {
            var response = await _authService.ActivateUser(userId);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Deactivate user account (SuperAdmin only)
        /// </summary>
        [Authorize(Roles = RoleConstants.SuperAdmin)]
        [HttpPatch("users/{userId}/deactivate")]
        public async Task<ActionResult<ApiResponseDto<MessageResponseDto>>> DeactivateUser(string userId)
        {
            var response = await _authService.DeactivateUser(userId);
            return response.ToApiResponse();
        }
    }
}
