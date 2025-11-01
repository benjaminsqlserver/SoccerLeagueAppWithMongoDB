using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.UserSessions.Commands.TerminateSession
{
    public class TerminateSessionCommandHandler : IRequestHandler<TerminateSessionCommand, Result<bool>>
    {
        private readonly IUserSessionRepository _repository;

        public TerminateSessionCommandHandler(IUserSessionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(TerminateSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _repository.GetByIdAsync(request.TerminateData.SessionId);

            if (session == null)
            {
                return Result<bool>.Failure("Session not found");
            }

            if (!session.IsActive)
            {
                return Result<bool>.Failure("Session is already terminated");
            }

            var result = await _repository.TerminateSessionAsync(
                request.TerminateData.SessionId,
                request.TerminateData.TerminationReason);

            if (!result)
            {
                return Result<bool>.Failure("Failed to terminate session");
            }

            return Result<bool>.Success(true);
        }
    }
}
