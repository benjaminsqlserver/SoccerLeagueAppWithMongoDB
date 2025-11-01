using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;
using SoccerLeague.Application.DTOs.Auth;
using SoccerLeague.Domain.Constants;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;

namespace SoccerLeague.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IGoogleAuthService _googleAuthService;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IJwtService jwtService,
            IEmailService emailService,
            IUserSessionRepository sessionRepository,
            IGoogleAuthService googleAuthService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _sessionRepository = sessionRepository;
            _googleAuthService = googleAuthService;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto registerDto, string? ipAddress = null, string? userAgent = null)
        {
            // Check if email exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return Result<AuthResponseDto>.Failure("Email already exists");
            }

            // Get default role
            var defaultRole = await _roleRepository.GetByNameAsync(SystemRoles.User);
            if (defaultRole == null)
            {
                return Result<AuthResponseDto>.Failure("System configuration error");
            }

            // Create user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = registerDto.Email.ToLower(),
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                ProfilePicture = registerDto.ProfilePicture,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                AuthProvider = AuthenticationProvider.Local,
                EmailConfirmed = false,
                IsActive = true,
                RoleIds = new List<string> { defaultRole.Id },
                EmailVerificationToken = Guid.NewGuid().ToString(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24),
                CreatedDate = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _emailService.SendEmailVerificationAsync(user.Email, user.FirstName, user.EmailVerificationToken!);

            return await GenerateAuthResponse(user, new List<string> { SystemRoles.User }, ipAddress, userAgent);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto, string? ipAddress = null, string? userAgent = null)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials");
            }

            if (user.IsLockedOut)
            {
                return Result<AuthResponseDto>.Failure($"Account locked until {user.LockoutEnd}");
            }

            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Account is inactive");
            }

            await _userRepository.ResetAccessFailedCountAsync(user.Id);
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var roleNames = await GetUserRoleNames(user.RoleIds);
            return await GenerateAuthResponse(user, roleNames, ipAddress, userAgent, loginDto.RememberMe);
        }

        public async Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginDto googleLoginDto, string? ipAddress = null, string? userAgent = null)
        {
            var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(googleLoginDto.IdToken);
            if (googleUser == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid Google token");
            }

            var user = await _userRepository.GetByGoogleIdAsync(googleUser.Sub);

            if (user == null)
            {
                user = await _userRepository.GetByEmailAsync(googleUser.Email);

                if (user == null)
                {
                    // Create new user
                    var defaultRole = await _roleRepository.GetByNameAsync(SystemRoles.User);
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = googleUser.Email.ToLower(),
                        FirstName = googleUser.GivenName,
                        LastName = googleUser.FamilyName,
                        GoogleId = googleUser.Sub,
                        ProfilePictureUrl = googleUser.Picture,
                        AuthProvider = AuthenticationProvider.Google,
                        EmailConfirmed = googleUser.EmailVerified,
                        IsActive = true,
                        RoleIds = new List<string> { defaultRole!.Id },
                        CreatedDate = DateTime.UtcNow
                    };
                    await _userRepository.AddAsync(user);
                }
                else
                {
                    // Link existing account
                    user.GoogleId = googleUser.Sub;
                    user.AuthProvider = AuthenticationProvider.Google;
                    if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                    {
                        user.ProfilePictureUrl = googleUser.Picture;
                    }
                    await _userRepository.UpdateAsync(user);
                }
            }

            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var roleNames = await GetUserRoleNames(user.RoleIds);
            return await GenerateAuthResponse(user, roleNames, ipAddress, userAgent);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, string? ipAddress = null, string? userAgent = null)
        {
            var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken);

            if (session == null || !session.IsActive || session.SessionExpiryDate < DateTime.UtcNow)
            {
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token");
            }

            var user = await _userRepository.GetByIdAsync(session.UserId);
            if (user == null || !user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("User not found");
            }

            var roleNames = await GetUserRoleNames(user.RoleIds);
            return await GenerateAuthResponse(user, roleNames, ipAddress, userAgent);
        }

        public async Task<Result<bool>> LogoutAsync(string userId, string? refreshToken = null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken);
                if (session != null)
                {
                    await _sessionRepository.TerminateSessionAsync(session.Id, "User logout");
                }
            }
            else
            {
                await _sessionRepository.TerminateAllUserSessionsAsync(userId, "User logout");
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return Result<bool>.Success(true); // Don't reveal user existence
            }

            var resetToken = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, resetToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.GetByEmailAsync(resetPasswordDto.Email);

            if (user == null ||
                string.IsNullOrEmpty(user.PasswordResetToken) ||
                user.PasswordResetToken != resetPasswordDto.Token ||
                !user.PasswordResetTokenExpiry.HasValue ||
                user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Invalid or expired reset token");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            await _sessionRepository.TerminateAllUserSessionsAsync(user.Id, "Password reset");
            await _emailService.SendPasswordChangedNotificationAsync(user.Email, user.FirstName);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto verifyEmailDto)
        {
            var user = await _userRepository.GetByEmailAsync(verifyEmailDto.Email);

            if (user == null ||
                user.EmailConfirmed ||
                string.IsNullOrEmpty(user.EmailVerificationToken) ||
                user.EmailVerificationToken != verifyEmailDto.Token ||
                !user.EmailVerificationTokenExpiry.HasValue ||
                user.EmailVerificationTokenExpiry.Value < DateTime.UtcNow)
            {
                return Result<bool>.Failure("Invalid or expired verification token");
            }

            await _userRepository.ConfirmEmailAsync(user.Id);
            await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> ResendVerificationEmailAsync(ResendVerificationEmailDto resendDto)
        {
            var user = await _userRepository.GetByEmailAsync(resendDto.Email);

            if (user == null)
            {
                return Result<bool>.Success(true); // Don't reveal user existence
            }

            if (user.EmailConfirmed)
            {
                return Result<bool>.Failure("Email already verified");
            }

            var newToken = Guid.NewGuid().ToString();
            user.EmailVerificationToken = newToken;
            user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendEmailVerificationAsync(user.Email, user.FirstName, newToken);
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> RevokeTokenAsync(string refreshToken)
        {
            var session = await _sessionRepository.GetByRefreshTokenAsync(refreshToken);
            if (session != null)
            {
                await _sessionRepository.TerminateSessionAsync(session.Id, "Token revoked");
            }
            return Result<bool>.Success(true);
        }

        private async Task<Result<AuthResponseDto>> GenerateAuthResponse(User user, List<string> roleNames, string? ipAddress, string? userAgent, bool rememberMe = false)
        {
            var accessToken = _jwtService.GenerateAccessToken(user, roleNames);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var tokenExpiry = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = tokenExpiry;
            await _userRepository.UpdateAsync(user);

            var session = new UserSession
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                RefreshToken = refreshToken,
                TokenId = Guid.NewGuid().ToString(),
                SessionStartDate = DateTime.UtcNow,
                SessionExpiryDate = tokenExpiry,
                LastActivityDate = DateTime.UtcNow,
                IsActive = true,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                CreatedDate = DateTime.UtcNow
            };
            await _sessionRepository.AddAsync(session);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Roles = roleNames,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = _jwtService.GetTokenExpiry(),
                EmailConfirmed = user.EmailConfirmed
            });
        }

        private async Task<List<string>> GetUserRoleNames(List<string> roleIds)
        {
            var roleNames = new List<string>();
            foreach (var roleId in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null && role.IsActive)
                {
                    roleNames.Add(role.Name);
                }
            }
            return roleNames;
        }
    }
}
