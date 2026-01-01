using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NazarMahal.Core.Abstractions;
using NazarMahal.Infrastructure.Data;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;

namespace NazarMahal.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangeUserPasswordRequestDto changeUserPasswordRequestDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            var result = await _userManager.ChangePasswordAsync(
                user, changeUserPasswordRequestDto.CurrentPassword, changeUserPasswordRequestDto.NewPassword);

            return result.Succeeded;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<bool> DisableUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.IsDisabled = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<UserResponseDto> AddUserAsync(CreateNewUserRequestDto createNewUserRequestDto)
        {
            var user = _mapper.Map<ApplicationUser>(createNewUserRequestDto);
            var result = await _userManager.CreateAsync(user, createNewUserRequestDto.Password);

            return result.Succeeded ? _mapper.Map<UserResponseDto>(user) : null;
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
            user.Address = address;
            user.IsDisabled = isDisabled;

            if (profilePicture != null)
            {
                var uploadsFolder = Path.Combine("wwwroot", "uploads", "profile-pictures");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = $"{Guid.NewGuid()}_{profilePicture.FileName}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }

                user.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Failed to update user.");
            }

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IList<UserResponseDto>> GetUserListByRoleId(string roleName)
        {

            var user = await _userManager.GetUsersInRoleAsync(roleName);
            return _mapper.Map<IList<UserResponseDto>>(user);


        }
    }
}
