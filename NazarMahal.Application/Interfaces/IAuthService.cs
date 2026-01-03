using NazarMahal.Application.Models;
using NazarMahal.Application.ResponseDto.AuthResponseDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Abstractions;
using NazarMahal.Core.ActionResponses;

namespace NazarMahal.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ActionResponse<RegisterResponseDto>> Register(RegisterModel registerModel, IRequestContextAccessor requestContext);
        public Task<ActionResponse<LoginResponseDto>> Login(LoginModel loginModel);
        public Task<ActionResponse<TokenResponseDto>> RegisterAdmin(RegisterModel registerAdminModel);
        public Task<ActionResponse<MessageResponseDto>> ForgotPassword(string email, IRequestContextAccessor requestContext);
        public Task<ActionResponse<MessageResponseDto>> ResetPassword(ResetPasswordRequest model);
        public Task<ActionResponse<TokenResponseDto>> RefreshToken(string userId);
        public Task<ActionResponse<UserProfileResponseDto>> GetCurrentUser(string userId);
        public Task<ActionResponse<MessageResponseDto>> UpdateUserRole(string userId, UpdateUserRoleRequest model);
        public Task<ActionResponse<MessageResponseDto>> ActivateUser(string userId);
        public Task<ActionResponse<MessageResponseDto>> DeactivateUser(string userId);
        public Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllUsers();
        public Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllCustomers();
        public Task<ActionResponse<UserProfileResponseDto>> GetUserById(string userId);
        public Task<ActionResponse<MessageResponseDto>> ConfirmEmail(string userId, string token);
        public Task<ActionResponse<MessageResponseDto>> ValidateResetToken(string userId, string token);
        public Task<ActionResponse<MessageResponseDto>> Logout(string userId);
    }
}
