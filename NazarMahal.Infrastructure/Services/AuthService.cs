using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Models;
using NazarMahal.Application.ResponseDto.AuthResponseDto;
using NazarMahal.Application.ResponseDto.UserResponseDto;
using NazarMahal.Core.Abstractions;
using NazarMahal.Core.ActionResponses;
using NazarMahal.Core.Common;
using NazarMahal.Core.Enums;
using NazarMahal.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NazarMahal.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;


        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IConfiguration configuration,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        public async Task<ActionResponse<RegisterResponseDto>> Register(RegisterModel model, IRequestContextAccessor requestContext)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<RegisterResponseDto>("Invalid data.");

                if (model.Password != model.ConfirmPassword)
                    return new FailActionResponse<RegisterResponseDto>("Passwords do not match.");

                // Validate Pakistani phone number format
                if (string.IsNullOrEmpty(model.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(model.PhoneNumber, @"^03\d{9}$"))
                    return new FailActionResponse<RegisterResponseDto>("Phone number must be 11 digits.");

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return new FailActionResponse<RegisterResponseDto>("Email already in use.");

                var user = new ApplicationUser
                {
                    FullName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = RoleConstants.Customer,
                    DateCreated = PakistanTimeHelper.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return new FailActionResponse<RegisterResponseDto>(result.Errors.Select(e => e.Description).ToList());

                if (!await _roleManager.RoleExistsAsync(RoleConstants.Customer))
                {
                    var customerRole = new ApplicationRole { Name = RoleConstants.Customer };
                    await _roleManager.CreateAsync(customerRole);
                }

                await _userManager.AddToRoleAsync(user, RoleConstants.Customer);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, RoleConstants.Customer),
                    new Claim("UserType", user.UserType)
                };

                foreach (var claim in claims)
                {
                    await _userManager.AddClaimAsync(user, claim);
                }

                await _userManager.GetClaimsAsync(user);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = requestContext.BuildUrl($"api/auth/ConfirmEmail?userId={user.Id}&token={Uri.EscapeDataString(token)}");

                if (confirmationLink == null)
                {
                    return new FailActionResponse<RegisterResponseDto>("Confirmation link is invalid");
                }
                await _notificationService.SendAccountConfirmationEmail(new List<string> { user.Email }, confirmationLink);

                return new OkActionResponse<RegisterResponseDto>(
                    new RegisterResponseDto { Message = "User registered successfully. Please check your email to confirm your account." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<RegisterResponseDto>($"Error occurred in Register: {ex.Message}");
            }
        }

        public async Task<ActionResponse<LoginResponseDto>> Login(LoginModel model)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<LoginResponseDto>("Invalid data.");

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                    return new FailActionResponse<LoginResponseDto>("Invalid credentials.");

                var userClaims = await _userManager.GetClaimsAsync(user);
                var userType = user.UserType;
                var isEmailConfirmed = user.EmailConfirmed;
                if (userType == RoleConstants.Customer || string.IsNullOrEmpty(userType))
                {
                    if (!isEmailConfirmed)
                    {
                        return new FailActionResponse<LoginResponseDto>("Please confirm your email before continuing.");
                    }
                }

                var userId = user.Id;
                var userFullName = user.FullName;
                var userEmail = user.Email;
                var isUserDisabled = user.IsDisabled;

                if (user.IsDisabled)
                {
                    return new FailActionResponse<LoginResponseDto>("Login Failed. Your account is disabled.");
                }

                var token = GenerateJwtToken(user, userClaims);

                var loginResponse = new LoginResponseDto
                {
                    Token = token,
                    UserId = userId.ToString(),
                    UserFullName = userFullName ?? string.Empty,
                    UserEmail = userEmail ?? string.Empty,
                    UserType = userType ?? RoleConstants.Customer,
                    Claims = userClaims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }),
                    Message = "Login successful."
                };

                return new OkActionResponse<LoginResponseDto>(loginResponse);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<LoginResponseDto>($"Error occurred in Login: {ex.Message}");
            }
        }

        public async Task<ActionResponse<TokenResponseDto>> RegisterAdmin(RegisterModel model)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<TokenResponseDto>("Invalid data.");

                if (model.Password != model.ConfirmPassword)
                    return new FailActionResponse<TokenResponseDto>("Passwords do not match.");

                // Validate Pakistani phone number format
                if (string.IsNullOrEmpty(model.PhoneNumber) || !System.Text.RegularExpressions.Regex.IsMatch(model.PhoneNumber, @"^03\d{9}$"))
                    return new FailActionResponse<TokenResponseDto>("Phone number must be 11 digits.");

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return new FailActionResponse<TokenResponseDto>("Email already in use.");

                var adminUser = new ApplicationUser
                {
                    FullName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserType = RoleConstants.Admin,
                    DateCreated = PakistanTimeHelper.Now
                };

                var result = await _userManager.CreateAsync(adminUser, model.Password);
                if (!result.Succeeded)
                    return new FailActionResponse<TokenResponseDto>(result.Errors.Select(e => e.Description).ToList());

                if (!await _roleManager.RoleExistsAsync(RoleConstants.Admin))
                {
                    var adminRole = new ApplicationRole { Name = RoleConstants.Admin };
                    await _roleManager.CreateAsync(adminRole);
                }

                await _userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, adminUser.FullName),
                    new Claim(ClaimTypes.Email, adminUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, RoleConstants.Admin),
                    new Claim("UserType", adminUser.UserType)
                };

                foreach (var claim in claims)
                {
                    await _userManager.AddClaimAsync(adminUser, claim);
                }

                var userClaims = await _userManager.GetClaimsAsync(adminUser);

                var token = GenerateJwtToken(adminUser, userClaims);

                return new OkActionResponse<TokenResponseDto>(
                    new TokenResponseDto { Token = token, Message = "Admin User registered successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<TokenResponseDto>($"Error occurred in RegisterAdmin: {ex.Message}");
            }
        }

        public string GenerateJwtToken(ApplicationUser user, IList<Claim> userClaims)
        {

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("FullName", user.FullName ?? string.Empty)
                };

            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ActionResponse<MessageResponseDto>> ForgotPassword(string email, IRequestContextAccessor requestContext)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return new FailActionResponse<MessageResponseDto>("Email is required.");
                }

                var normalizedEmail = _userManager.NormalizeEmail(email);
                var user = await _userManager.FindByEmailAsync(normalizedEmail);

                // Always return success message to prevent email enumeration attacks
                if (user == null)
                {
                    return new OkActionResponse<MessageResponseDto>(
                        new MessageResponseDto { Message = "If an account with the provided email exists, a password reset link will be sent." });
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Still send email, but don't reveal that email is not confirmed
                    return new OkActionResponse<MessageResponseDto>(
                        new MessageResponseDto { Message = "If an account with the provided email exists, a password reset link will be sent." });
                }

                // Generate password reset token (expires in 10 minutes as configured)
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Get environment-specific frontend URL from configuration or use a default
                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:4200";

                // Create frontend reset URL
                var resetUrl = $"{frontendUrl}/reset-password?userId={user.Id}&token={Uri.EscapeDataString(passwordResetToken)}";

                // Send password reset email using NotificationService
                await _notificationService.SendPasswordResetEmail(user.Email ?? throw new InvalidOperationException("User email is null"), resetUrl, user.FullName ?? "User");

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "If an account with the provided email exists, a password reset link will be sent to your email address. The link will expire in 10 minutes." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ForgotPassword: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<MessageResponseDto>("Invalid data.");

                if (model.NewPassword != model.ConfirmPassword)
                    return new FailActionResponse<MessageResponseDto>("Passwords do not match.");

                if (string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.Token))
                    return new FailActionResponse<MessageResponseDto>("Invalid token or userId.");

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (!result.Succeeded)
                    return new FailActionResponse<MessageResponseDto>(result.Errors.Select(e => e.Description).ToList());

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ResetPassword: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> ChangePassword(string userId, ChangePasswordRequest model)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<MessageResponseDto>("Invalid data.");

                if (model.NewPassword != model.ConfirmPassword)
                    return new FailActionResponse<MessageResponseDto>("Passwords do not match.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                    return new FailActionResponse<MessageResponseDto>(result.Errors.Select(e => e.Description).ToList());

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ChangePassword: {ex.Message}");
            }
        }

        public async Task<ActionResponse<TokenResponseDto>> RefreshToken(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<TokenResponseDto>("User not found.");

                if (user.IsDisabled)
                    return new FailActionResponse<TokenResponseDto>("User account is disabled.");

                var userClaims = await _userManager.GetClaimsAsync(user);
                var token = GenerateJwtToken(user, userClaims);

                return new OkActionResponse<TokenResponseDto>(
                    new TokenResponseDto
                    {
                        Token = token,
                        Message = "Token refreshed successfully."
                    });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<TokenResponseDto>($"Error occurred in RefreshToken: {ex.Message}");
            }
        }

        public async Task<ActionResponse<UserProfileResponseDto>> GetCurrentUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<UserProfileResponseDto>("User not found.");

                var roles = await _userManager.GetRolesAsync(user);

                var userProfile = new UserProfileResponseDto
                {
                    UserId = user.Id.ToString(),
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType ?? string.Empty,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsDisabled = user.IsDisabled,
                    DateCreated = user.DateCreated
                };

                return new OkActionResponse<UserProfileResponseDto>(userProfile);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<UserProfileResponseDto>($"Error occurred in GetCurrentUser: {ex.Message}");
            }
        }

        public async Task<ActionResponse<UserProfileResponseDto>> UpdateProfile(string userId, UpdateProfileRequest model)
        {
            try
            {
                if (model == null)
                    return new FailActionResponse<UserProfileResponseDto>("Invalid data.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<UserProfileResponseDto>("User not found.");

                // Validate Pakistani phone number format if provided
                if (!string.IsNullOrEmpty(model.PhoneNumber) && !System.Text.RegularExpressions.Regex.IsMatch(model.PhoneNumber, @"^03\d{9}$"))
                    return new FailActionResponse<UserProfileResponseDto>("Phone number must be 11 digits.");

                user.FullName = model.FullName ?? user.FullName;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return new FailActionResponse<UserProfileResponseDto>(result.Errors.Select(e => e.Description).ToList());

                var roles = await _userManager.GetRolesAsync(user);
                var updatedProfile = new UserProfileResponseDto
                {
                    UserId = user.Id.ToString(),
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType ?? string.Empty,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsDisabled = user.IsDisabled,
                    DateCreated = user.DateCreated
                };

                return new OkActionResponse<UserProfileResponseDto>(updatedProfile, "Profile updated successfully.");
            }
            catch (Exception ex)
            {
                return new FailActionResponse<UserProfileResponseDto>($"Error occurred in UpdateProfile: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> UpdateUserRole(string userId, UpdateUserRoleRequest model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.NewRole))
                    return new FailActionResponse<MessageResponseDto>("Invalid data.");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");

                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!await _roleManager.RoleExistsAsync(model.NewRole))
                {
                    var role = new ApplicationRole { Name = model.NewRole };
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(user, model.NewRole);

                // Update UserType in database (source of truth)
                user.UserType = model.NewRole;
                await _userManager.UpdateAsync(user);

                // Update UserType claim
                var claims = await _userManager.GetClaimsAsync(user);
                var userTypeClaim = claims.FirstOrDefault(c => c.Type == "UserType");
                if (userTypeClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, userTypeClaim);
                }

                var newUserTypeClaim = new System.Security.Claims.Claim("UserType", model.NewRole);
                await _userManager.AddClaimAsync(user, newUserTypeClaim);

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = $"User role updated to {model.NewRole} successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in UpdateUserRole: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> ActivateUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");

                user.IsDisabled = false;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return new FailActionResponse<MessageResponseDto>(result.Errors.Select(e => e.Description).ToList());

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "User activated successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ActivateUser: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> DeactivateUser(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");

                user.IsDisabled = true;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return new FailActionResponse<MessageResponseDto>(result.Errors.Select(e => e.Description).ToList());

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "User deactivated successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in DeactivateUser: {ex.Message}");
            }
        }

        public async Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllUsers()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var userList = new List<UserListResponseDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userList.Add(new UserListResponseDto
                    {
                        UserId = user.Id.ToString(),
                        FullName = user.FullName ?? string.Empty,
                        Email = user.Email ?? string.Empty,
                        PhoneNumber = user.PhoneNumber ?? string.Empty,
                        UserType = user.UserType ?? string.Empty,
                        IsEmailConfirmed = user.EmailConfirmed,
                        IsDisabled = user.IsDisabled,
                        DateCreated = user.DateCreated
                    });
                }

                return new OkActionResponse<IEnumerable<UserListResponseDto>>(userList);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<UserListResponseDto>>($"Error occurred in GetAllUsers: {ex.Message}");
            }
        }

        public async Task<ActionResponse<IEnumerable<UserListResponseDto>>> GetAllCustomers()
        {
            try
            {
                var customers = await _userManager.GetUsersInRoleAsync(RoleConstants.Customer);
                var customerList = new List<UserListResponseDto>();

                foreach (var customer in customers)
                {
                    customerList.Add(new UserListResponseDto
                    {
                        UserId = customer.Id.ToString(),
                        FullName = customer.FullName ?? string.Empty,
                        Email = customer.Email ?? string.Empty,
                        PhoneNumber = customer.PhoneNumber ?? string.Empty,
                        UserType = RoleConstants.Customer,
                        IsEmailConfirmed = customer.EmailConfirmed,
                        IsDisabled = customer.IsDisabled,
                        DateCreated = customer.DateCreated
                    });
                }

                return new OkActionResponse<IEnumerable<UserListResponseDto>>(customerList);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<IEnumerable<UserListResponseDto>>($"Error occurred in GetAllCustomers: {ex.Message}");
            }
        }

        public async Task<ActionResponse<UserProfileResponseDto>> GetUserById(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new NotFoundActionResponse<UserProfileResponseDto>("User not found.");

                var roles = await _userManager.GetRolesAsync(user);

                var userProfile = new UserProfileResponseDto
                {
                    UserId = user.Id.ToString(),
                    FullName = user.FullName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    UserType = user.UserType ?? string.Empty,
                    IsEmailConfirmed = user.EmailConfirmed,
                    IsDisabled = user.IsDisabled,
                    DateCreated = user.DateCreated
                };

                return new OkActionResponse<UserProfileResponseDto>(userProfile);
            }
            catch (Exception ex)
            {
                return new FailActionResponse<UserProfileResponseDto>($"Error occurred in GetUserById: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return new FailActionResponse<MessageResponseDto>("Invalid confirmation link.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new OkActionResponse<MessageResponseDto>(
                        new MessageResponseDto { Message = "Email confirmed successfully! You can now log in." });
                }
                else
                {
                    return new FailActionResponse<MessageResponseDto>(result.Errors.Select(e => e.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ConfirmEmail: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> ValidateResetToken(string userId, string token)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                {
                    return new FailActionResponse<MessageResponseDto>("Invalid token or userId.");
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundActionResponse<MessageResponseDto>("Invalid user.");
                }

                var isTokenValid = await _userManager.VerifyUserTokenAsync(
                    user,
                    _userManager.Options.Tokens.PasswordResetTokenProvider,
                    "ResetPassword",
                    token);

                if (!isTokenValid)
                {
                    return new FailActionResponse<MessageResponseDto>("Invalid or expired token.");
                }

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "Token is valid." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in ValidateResetToken: {ex.Message}");
            }
        }

        public async Task<ActionResponse<MessageResponseDto>> Logout(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundActionResponse<MessageResponseDto>("User not found.");
                }


                // For JWT-based auth, we don't need to do anything on the server side
                // The frontend will remove the token from localStorage
                // If you want to implement token blacklisting in the future, you can do it here

                return new OkActionResponse<MessageResponseDto>(
                    new MessageResponseDto { Message = "Logged out successfully." });
            }
            catch (Exception ex)
            {
                return new FailActionResponse<MessageResponseDto>($"Error occurred in Logout: {ex.Message}");
            }
        }
    }
}
