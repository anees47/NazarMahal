using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Common;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Enums;

namespace NazarMahal.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Get all users
        /// </summary>
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserListResponseDto>>>> GetUsers()
        {
            var response = await _userService.GetAllUserAsync();
            return response.ToApiResponse();
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUser(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> CreateUser([FromBody] CreateNewUserRequestDto request)
        {
            var response = await _userService.CreateNewUser(request);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update user information
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            request.UserId = id;
            var response = await _userService.UpdateUserInfoByIdAsync(request);
            return response.ToApiResponse();
        }

        /// <summary>
        /// Update user status/properties (disable, activate, etc.)
        /// Body:
        /// {
        ///   "isDisabled": true/false
        /// }
        /// </summary>
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequestDto request)
        {
            if (request.IsDisabled)
            {
                var response = await _userService.DisableUserByIdAsync(id);
                return response.ToApiResponse();
            }

            var enableResponse = new OkActionResponse<bool>(true, "User status updated successfully");
            return enableResponse.ToApiResponse();
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPatch("{id}/password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(int id, [FromBody] ChangeUserPasswordRequestDto request)
        {
            var response = await _userService.ChangePasswordByIdAsync(id, request);
            return response.ToApiResponse();
        }
    }
}
