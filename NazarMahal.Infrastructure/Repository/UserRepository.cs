using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NazarMahal.Infrastructure.Data;
using NazarMahal.Infrastructure.Mappers;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Application.Interfaces;

namespace NazarMahal.Infrastructure.Repository
{
    public class UserRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IFileStorage fileStorage) : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly IFileStorage _fileStorage = fileStorage;

        public async Task<bool> ChangePasswordAsync(int userId, ChangeUserPasswordRequestDto changeUserPasswordRequestDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(
                user, changeUserPasswordRequestDto.CurrentPassword, changeUserPasswordRequestDto.NewPassword);

            return result.Succeeded;
        }

        public async Task<bool> VerifyCurrentPasswordAsync(int userId, string currentPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, currentPassword);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.ToUserResponseDtoList();
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user.ToUserResponseDto();
        }

        public async Task<bool> DisableUserStatusAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.IsDisabled = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> EnableUserStatusAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            user.IsDisabled = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


        public async Task<UserResponseDto> AddUserAsync(CreateNewUserRequestDto createNewUserRequestDto)
        {
            var user = createNewUserRequestDto.ToApplicationUser();
            var result = await _userManager.CreateAsync(user, createNewUserRequestDto.Password);

            return result.Succeeded ? user.ToUserResponseDto() : null;
        }

        public async Task<UserResponseDto> UpdateUserInfoAsync(int userId, string fullname, string email, string address, bool isDisabled, IFormFile? profilePicture)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.FullName = fullname;
            user.Email = email;
            user.UserName = email;
            user.Address = address;
            user.IsDisabled = isDisabled;

            if (profilePicture != null)
            {
                // Validate content type
                var fileContentTypeResult = NazarMahal.Application.Common.FileContentType.Create(profilePicture.ContentType);
                if (!fileContentTypeResult.IsSuccessful || !fileContentTypeResult.Payload.IsImage())
                {
                    throw new InvalidOperationException("Invalid profile picture. Only image files are allowed.");
                }

                var savedRelativePath = await _fileStorage.SaveFileAsync(profilePicture, Path.Combine("uploads", "profile-pictures"));
                user.ProfilePictureUrl = savedRelativePath.StartsWith("/") ? savedRelativePath : "/" + savedRelativePath;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update user.");
            }

            return user.ToUserResponseDto();
        }

        public async Task<IList<UserResponseDto>> GetUserListByRoleId(string roleName)
        {
            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return users.ToUserResponseDtoList().ToList();
        }
    }
}
