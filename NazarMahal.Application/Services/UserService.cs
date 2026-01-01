using AutoMapper;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.ActionResponses;

namespace NazarMahal.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ActionResponse<UserResponseDto>> GetUserByIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    return new FailActionResponse<UserResponseDto>("Invalid user ID. User ID must be greater than 0.");

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) return new NotFoundActionResponse<UserResponseDto>("User not found");

                var userResult = _mapper.Map<UserResponseDto>(user);
                return new OkActionResponse<UserResponseDto>(userResult);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<UserResponseDto>($"Error occurred in {nameof(GetUserByIdAsync)}: {ex.Message}");
            }
        }

        public async Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllUserAsync()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                if (users == null || !users.Any())
                    return new NotFoundActionResponse<IEnumerable<UserListResponseDto>>("No users found");

                var userList = _mapper.Map<IEnumerable<UserListResponseDto>>(users);
                return new OkActionResponse<IEnumerable<UserListResponseDto>>(userList);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<UserListResponseDto>>($"Error occurred in {nameof(GetAllUserAsync)}: {ex.Message}");
            }
        }

        public async Task<ActionResponse<bool>> ChangePasswordByIdAsync(int userId, ChangeUserPasswordRequestDto changePasswordRequest)
        {
            try
            {
                if (userId <= 0)
                    return new FailActionResponse<bool>("Invalid user ID. User ID must be greater than 0.");

                if (changePasswordRequest == null)
                    return new FailActionResponse<bool>("Request body cannot be null.");

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) return new NotFoundActionResponse<bool>("Requested user is not found");

                if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmPassword)
                    return new FailActionResponse<bool>(false, "New Password and confirm password do not match");
                var isPasswordChanged = await _userRepository.ChangePasswordAsync(userId, changePasswordRequest);
                if (!isPasswordChanged)
                    return new FailActionResponse<bool>("Failed to change the password");

                return new OkActionResponse<bool>(true, "Password changed successfully");
            }
            catch (Exception)
            {
                return new FailActionResponse<bool>($"Error occurred in UserService.");
            }
        }

        public async Task<ActionResponse<bool>> DisableUserByIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    return new FailActionResponse<bool>("Invalid user ID. User ID must be greater than 0.");

                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null) return new NotFoundActionResponse<bool>("User not found");


                var isUpdated = await _userRepository.DisableUserAsync(userId);
                if (!isUpdated)
                    return new FailActionResponse<bool>("Failed to disable user");

                return new OkActionResponse<bool>(true, "User disabled successfully");
            }
            catch (Exception)
            {
                return new FailActionResponse<bool>($"Error occurred in UserService.DisableUserByIdAsync");
            }
        }



        public async Task<ActionResponse<UserResponseDto>> CreateNewUser(CreateNewUserRequestDto createNewUserRequestDto)
        {
            try
            {
                if (createNewUserRequestDto == null)
                    return new FailActionResponse<UserResponseDto>("Request body cannot be null.");

                var existingUser = _userRepository.GetAllUsersAsync()
                                    .Result.FirstOrDefault(u => u.Email == createNewUserRequestDto.Email);
                if (existingUser != null)
                {
                    return new FailActionResponse<UserResponseDto>("User with this email already exists");
                }

                var newUser = _mapper.Map<CreateNewUserRequestDto>(createNewUserRequestDto);

                var createdUser = await _userRepository.AddUserAsync(newUser);

                if (createdUser == null)
                {
                    return new FailActionResponse<UserResponseDto>("Failed to create new user");
                }

                var userResult = _mapper.Map<UserResponseDto>(createdUser);
                return new OkActionResponse<UserResponseDto>(userResult, "User created successfully");
            }
            catch (Exception)
            {
                return new FailActionResponse<UserResponseDto>($"Error occurred in UserService.CreateNewUser");
            }
        }
        public async Task<ActionResponse<UserResponseDto>> UpdateUserInfoByIdAsync(UpdateUserRequestDto updateUserRequest)
        {
            if (updateUserRequest == null)
                return new FailActionResponse<UserResponseDto>("Request body cannot be null.");

            if (updateUserRequest.UserId <= 0)
                return new FailActionResponse<UserResponseDto>("Invalid user ID. User ID must be greater than 0.");

            try
            {
                var result = await _userRepository.UpdateUserInfoAsync(updateUserRequest.UserId, updateUserRequest.Fullname, updateUserRequest.Email, updateUserRequest.Address, updateUserRequest.IsDisabled, updateUserRequest.ProfilePicture);
                if (result == null) return new FailActionResponse<UserResponseDto>("An Error Occured");
                var updateResult = _mapper.Map<UserResponseDto>(result);
                return new OkActionResponse<UserResponseDto>(updateResult);
            }
            catch (Exception)
            {
                return new FailActionResponse<UserResponseDto>("An Error occured in UserService.UpdateUserInfoByIdAsync");
            }
        }
    }
}
