using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.Contracts.Services;
using SoccerLeague.Application.DTOs.Auth;

namespace SoccerLeague.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IJwtService _jwtService;
        private readonly IUserSessionRepository _sessionRepository;

        public RefreshTokenCommandHandler(
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

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var session = await _sessionRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (session == null || !session.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Invalid refresh token");
            }

            if (session.SessionExpiryDate < DateTime.UtcNow)
            {
                await _sessionRepository.TerminateSessionAsync(session.Id, "Token expired");
                return Result<AuthResponseDto>.Failure("Refresh token expired");
            }

            var user = await _userRepository.GetByIdAsync(session.UserId);
            if (user == null || !user.IsActive)
            {
                return Result<AuthResponseDto>.Failure("User not found or inactive");
            }

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

            // Generate new tokens
            var accessToken = _jwtService.GenerateAccessToken(user, roleNames);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Update user
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            // Update session
            session.RefreshToken = newRefreshToken;
            session.LastActivityDate = DateTime.UtcNow;
            await _sessionRepository.UpdateAsync(session);

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
                RefreshToken = newRefreshToken,
                TokenExpiry = _jwtService.GetTokenExpiry(),
                EmailConfirmed = user.EmailConfirmed
            });
        }
    }
}
