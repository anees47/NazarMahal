using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NazarMahal.API.Extensions;
using NazarMahal.Application.Common;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Models;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Enums;
using NazarMahal.Infrastructure.Services;

namespace NazarMahal.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<UserListResponseDto>>>> GetUsers()
        {
            var response = await userService.GetAllUserAsync();
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> GetUser(int id)
        {
            var response = await userService.GetUserByIdAsync(id);
            return response.ToApiResponse();
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> CreateUser([FromBody] CreateNewUserRequestDto request)
        {
            var response = await userService.CreateNewUser(request);
            return response.ToApiResponse();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<UserResponseDto>>> UpdateUser(int id, [FromBody] UpdateUserRequestDto request)
        {
            request.UserId = id;
            var response = await userService.UpdateUserInfoByIdAsync(request);
            return response.ToApiResponse();
        }
       
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.SuperAdmin}")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequestDto request)
        {
            var response = await userService.UpdateUserStatusAsync(id);
            return response.ToApiResponse();
        }

        [HttpPatch("{id}/password")]
        public async Task<ActionResult<ApiResponseDto<bool>>> ChangePassword(int id, [FromBody] ChangeUserPasswordRequestDto request)
        {
            var response = await userService.ChangePasswordByIdAsync(id, request);
            return response.ToApiResponse();
        }
    }
}
