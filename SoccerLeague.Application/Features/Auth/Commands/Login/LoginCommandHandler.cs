using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;
using SoccerLeague.Application.DTOs.Auth;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUserSessionRepository _sessionRepository;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IJwtService jwtService,
            IUserSessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var validator = new LoginCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<AuthResponseDto>.Failure(errors);
            }

            // Get user by email
            var user = await _userRepository.GetByEmailAsync(request.LoginDto.Email);

            if (user == null)
            {
                await _userRepository.IncrementAccessFailedCountAsync(user?.Id ?? "");
                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            // Check if user is locked out
            if (user.IsLockedOut)
            {
                return Result<AuthResponseDto>.Failure($"Account is locked until {user.LockoutEnd:yyyy-MM-dd HH:mm:ss} UTC");
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Account is inactive. Please contact administrator.");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.LoginDto.Password, user.PasswordHash))
            {
                await _userRepository.IncrementAccessFailedCountAsync(user.Id);

                // Check if should lock account (e.g., after 5 failed attempts)
                var updatedUser = await _userRepository.GetByIdAsync(user.Id);
                if (updatedUser != null && updatedUser.AccessFailedCount >= 5)
                {
                    await _userRepository.LockoutUserAsync(user.Id, DateTime.UtcNow.AddMinutes(30));
                    return Result<AuthResponseDto>.Failure("Account locked due to multiple failed login attempts. Try again in 30 minutes.");
                }

                return Result<AuthResponseDto>.Failure("Invalid email or password");
            }

            // Reset failed login count on successful login
            await _userRepository.ResetAccessFailedCountAsync(user.Id);

            // Get user roles
            var roleNames = new List<string>();
            foreach (var roleId in user.RoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role != null && role.IsActive)
                {
                    roleNames.Add(role.Name);
                }
            }

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(user, roleNames);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var tokenExpiry = request.LoginDto.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7);

            // Update user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = tokenExpiry;
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Create session
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
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                CreatedDate = DateTime.UtcNow
            };
            await _sessionRepository.AddAsync(session);

            // Return auth response
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
    }
}
