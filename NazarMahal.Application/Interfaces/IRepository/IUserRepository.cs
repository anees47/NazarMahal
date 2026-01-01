using Microsoft.AspNetCore.Http;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Abstractions;

namespace NazarMahal.Application.Interfaces.IRepository
{
    public interface IUserRepository
    {
        Task<UserResponseDto> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<bool> DisableUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangeUserPasswordRequestDto changeUserPasswordRequestDto);
        Task<UserResponseDto> AddUserAsync(CreateNewUserRequestDto createNewUserRequestDto);
        Task<UserResponseDto> UpdateUserInfoAsync(int userId, string fullname, string email, string address, bool isDisabled, IFormFile? profilePicture);

        Task<IList<UserResponseDto>> GetUserListByRoleId(string roleName);

    }
}
