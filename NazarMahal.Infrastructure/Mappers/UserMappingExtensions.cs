using NazarMahal.Application.RequestDto.UserRequestDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Infrastructure.Data;

namespace NazarMahal.Infrastructure.Mappers
{
    /// <summary>
    /// Custom mapping extensions for User-related DTOs
    /// </summary>
    public static class UserMappingExtensions
    {
        /// <summary>
        /// Map ApplicationUser to UserResponseDto
        /// </summary>
        public static UserResponseDto ToUserResponseDto(this ApplicationUser user)
        {
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserType = user.UserType,
                IsEmailConfirmed = user.EmailConfirmed,
                IsDisabled = user.IsDisabled,
                DateCreated = user.DateCreated
            };
        }

        /// <summary>
        /// Map ApplicationUser to UserListResponseDto
        /// </summary>
        public static UserListResponseDto ToUserListResponseDto(this ApplicationUser user)
        {
            if (user == null) return null;

            return new UserListResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                UserType = user.UserType,
                IsEmailConfirmed = user.EmailConfirmed,
                IsDisabled = user.IsDisabled,
                DateCreated = user.DateCreated
            };
        }

        /// <summary>
        /// Map IEnumerable of ApplicationUser to IEnumerable of UserResponseDto
        /// </summary>
        public static IEnumerable<UserResponseDto> ToUserResponseDtoList(this IEnumerable<ApplicationUser> users)
        {
            if (users == null) return Enumerable.Empty<UserResponseDto>();
            return users.Select(u => u.ToUserResponseDto());
        }

        /// <summary>
        /// Map IEnumerable of ApplicationUser to List of UserListResponseDto
        /// </summary>
        public static List<UserListResponseDto> ToUserListResponseDtoList(this IEnumerable<ApplicationUser> users)
        {
            if (users == null) return new List<UserListResponseDto>();
            return users.Select(u => u.ToUserListResponseDto()).ToList();
        }

        /// <summary>
        /// Map CreateNewUserRequestDto to ApplicationUser
        /// </summary>
        public static ApplicationUser ToApplicationUser(this CreateNewUserRequestDto request)
        {
            if (request == null) return null;

            return new ApplicationUser
            {
                FullName = request.Name,
                Email = request.Email,
                UserName = request.Email,
                Address = request.Address,
                ProfilePictureUrl = request.ProfilePictureUrl,
                IsDisabled = request.IsDisabled,
                DateCreated = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Map UserResponseDto to UpdateUserRequestDto
        /// </summary>
        public static UpdateUserRequestDto ToUpdateUserRequestDto(this UserResponseDto user)
        {
            if (user == null) return null;

            return new UpdateUserRequestDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Address = user.Address,
                IsDisabled = user.IsDisabled
            };
        }

        /// <summary>
        /// Map UserResponseDto to UserListResponseDto
        /// </summary>
        public static UserListResponseDto ToUserListResponseDto(this UserResponseDto user)
        {
            if (user == null) return null;

            return new UserListResponseDto
            {
                UserId = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                UserType = string.Empty,
                IsEmailConfirmed = false,
                IsDisabled = user.IsDisabled,
                DateCreated = DateTime.UtcNow
            };
        }
    }
}
