using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;
using SoccerLeague.Application.DTOs.Auth;
using SoccerLeague.Domain.Constants;
using SoccerLeague.Domain.Entities;
using SoccerLeague.Domain.Enums;

namespace SoccerLeague.Application.Features.Auth.Commands.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IEmailService _emailService;

        public GoogleLoginCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IJwtService jwtService,
            IGoogleAuthService googleAuthService,
            IUserSessionRepository sessionRepository,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
            _sessionRepository = sessionRepository;
            _emailService = emailService;
        }

        public async Task<Result<AuthResponseDto>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {
            var validator = new GoogleLoginCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<AuthResponseDto>.Failure(errors);
            }

            // Validate Google token
            var googleUser = await _googleAuthService.ValidateGoogleTokenAsync(request.GoogleLoginDto.IdToken);

            if (googleUser == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid Google token");
            }

            // Check if user exists by Google ID
            var user = await _userRepository.GetByGoogleIdAsync(googleUser.Sub);

            if (user == null)
            {
                // Check if user exists by email
                user = await _userRepository.GetByEmailAsync(googleUser.Email);

                if (user == null)
                {
                    // Create new user
                    var defaultRole = await _roleRepository.GetByNameAsync(SystemRoles.User);

                    if (defaultRole == null)
                    {
                        return Result<AuthResponseDto>.Failure("System configuration error");
                    }

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
                        RoleIds = new List<string> { defaultRole.Id },
                        CreatedDate = DateTime.UtcNow
                    };

                    await _userRepository.AddAsync(user);

                    // Send welcome email for new Google users
                    if (googleUser.EmailVerified)
                    {
                        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
                    }
                }
                else
                {
                    // Link existing account with Google
                    user.GoogleId = googleUser.Sub;
                    user.AuthProvider = AuthenticationProvider.Google;

                    if (string.IsNullOrEmpty(user.ProfilePictureUrl))
                    {
                        user.ProfilePictureUrl = googleUser.Picture;
                    }

                    if (!user.EmailConfirmed && googleUser.EmailVerified)
                    {
                        user.EmailConfirmed = true;
                    }
                    
                    await _userRepository.UpdateAsync(user);
                }
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Account is inactive. Please contact administrator.");
            }

            // Update last login
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

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
            var tokenExpiry = DateTime.UtcNow.AddDays(7);

            // Update user tokens
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = tokenExpiry;
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
