using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Auth.Commands.RevokeToken
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result<bool>>
    {
        private readonly IUserSessionRepository _sessionRepository;

        public RevokeTokenCommandHandler(IUserSessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            var validator = new RevokeTokenCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            // Find the session by refresh token
            var session = await _sessionRepository.GetByRefreshTokenAsync(request.RefreshToken);

            if (session == null)
            {
                return Result<bool>.Failure("Invalid refresh token");
            }

            // Verify the token belongs to the authenticated user
            if (session.UserId != request.UserId)
            {
                return Result<bool>.Failure("Unauthorized: Token does not belong to this user");
            }

            // Terminate the session
            var result = await _sessionRepository.TerminateSessionAsync(
                session.Id,
                "Token revoked by user");

            if (!result)
            {
                return Result<bool>.Failure("Failed to revoke token");
            }

            return Result<bool>.Success(true);
        }
    }
}
