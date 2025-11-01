using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _sessionRepository;

        public LogoutCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository sessionRepository)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            // Clear refresh token from user
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userRepository.UpdateAsync(user);

            // Terminate specific session if refresh token provided
            if (!string.IsNullOrEmpty(request.RefreshToken))
            {
                var session = await _sessionRepository.GetByRefreshTokenAsync(request.RefreshToken);
                if (session != null)
                {
                    await _sessionRepository.TerminateSessionAsync(session.Id, "User logout");
                }
            }
            else
            {
                // Terminate all user sessions
                await _sessionRepository.TerminateAllUserSessionsAsync(request.UserId, "User logout");
            }

            return Result<bool>.Success(true);
        }
    }
}
