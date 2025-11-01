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

namespace SoccerLeague.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IUserSessionRepository _sessionRepository;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IJwtService jwtService,
            IEmailService emailService,
            IUserSessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validator = new RegisterCommandValidator(_userRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<AuthResponseDto>.Failure(errors);
            }

            // Get default user role
            var defaultRole = await _roleRepository.GetByNameAsync(SystemRoles.User);
            if (defaultRole == null)
            {
                return Result<AuthResponseDto>.Failure("Default role not found. Please contact administrator.");
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.RegisterDto.Email.ToLower(),
                FirstName = request.RegisterDto.FirstName,
                LastName = request.RegisterDto.LastName,
                PhoneNumber = request.RegisterDto.PhoneNumber,
                ProfilePicture = request.RegisterDto.ProfilePicture,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.RegisterDto.Password),
                AuthProvider = AuthenticationProvider.Local,
                EmailConfirmed = false,
                IsActive = true,
                RoleIds = new List<string> { defaultRole.Id },
                EmailVerificationToken = Guid.NewGuid().ToString(),
                EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24),
                CreatedDate = DateTime.UtcNow
            };

            // Save user
            var createdUser = await _userRepository.AddAsync(user);

            // Send verification email
            await _emailService.SendEmailVerificationAsync(
                createdUser.Email,
                createdUser.FirstName,
                createdUser.EmailVerificationToken!);

            // Generate tokens
            var roles = new List<string> { SystemRoles.User };
            var accessToken = _jwtService.GenerateAccessToken(createdUser, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save refresh token
            createdUser.RefreshToken = refreshToken;
            createdUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(createdUser);

            // Create session
            var session = new UserSession
            {
                Id = Guid.NewGuid().ToString(),
                UserId = createdUser.Id,
                RefreshToken = refreshToken,
                TokenId = Guid.NewGuid().ToString(),
                SessionStartDate = DateTime.UtcNow,
                SessionExpiryDate = DateTime.UtcNow.AddDays(7),
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
                UserId = createdUser.Id,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                FullName = createdUser.FullName,
                ProfilePictureUrl = createdUser.ProfilePictureUrl,
                Roles = roles,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = _jwtService.GetTokenExpiry(),
                EmailConfirmed = createdUser.EmailConfirmed
            });
        }
    }
}
