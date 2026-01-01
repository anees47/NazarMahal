using NazarMahal.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Enums;

namespace NazarMahal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUserById(int userId)
        {
            var response = await _userService.GetUserByIdAsync(userId);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> CreateNewUser(CreateNewUserRequestDto createNewUserRequestDto)
        {
            var response = await _userService.CreateNewUser(createNewUserRequestDto);
            return response.ToApiResponse();
        }

        [HttpPut("Update/")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> UpdateUserInfoById(UpdateUserRequestDto updateUserRequest)
        {
            var response = await _userService.UpdateUserInfoByIdAsync(updateUserRequest);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserListResponseDto>>>> GetAllUser()
        {
            var response = await _userService.GetAllUserAsync();
            return response.ToApiResponse();

        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpDelete("Disable/{userId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DisableUserById(int userId)
        {
            var response = await _userService.DisableUserByIdAsync(userId);
            return response.ToApiResponse();
        }

        [HttpPut("ChangePassword/{userId}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePasswordById(int userId, ChangeUserPasswordRequestDto changePasswordRequest)
        {
            var response = await _userService.ChangePasswordByIdAsync(userId, changePasswordRequest);
            return response.ToApiResponse();
        }
    }
}
