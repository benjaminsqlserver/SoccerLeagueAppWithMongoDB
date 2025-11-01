using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.UserSessions.Commands.TerminateAllUserSessions
{
    public class TerminateAllUserSessionsCommandHandler : IRequestHandler<TerminateAllUserSessionsCommand, Result<bool>>
    {
        private readonly IUserSessionRepository _repository;

        public TerminateAllUserSessionsCommandHandler(IUserSessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(TerminateAllUserSessionsCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Result<bool>.Failure("User ID is required");
            }

            var result = await _repository.TerminateAllUserSessionsAsync(
                request.UserId,
                request.TerminationReason);

            if (!result)
            {
                return Result<bool>.Failure("Failed to terminate user sessions");
            }

            return Result<bool>.Success(true);
        }
    }
}
