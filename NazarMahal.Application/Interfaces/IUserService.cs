using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.ActionResponses;

namespace NazarMahal.Application.Interfaces
{
    public interface IUserService
    {
        Task<ActionResponse<UserResponseDto>> GetUserByIdAsync(int userId);
        Task<ActionResponse<UserResponseDto>> CreateNewUser(CreateNewUserRequestDto createNewUserRequestDto);
        Task<ActionResponse<UserResponseDto>> UpdateUserInfoByIdAsync(UpdateUserRequestDto updateUserRequest);
        Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllUserAsync();
        Task<ActionResponse<bool>> DisableUserByIdAsync(int userId);
        Task<ActionResponse<bool>> ChangePasswordByIdAsync(int userId, ChangeUserPasswordRequestDto changePasswordRequest);
    }
}
