using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.DeleteMatchStatus
{
    public class DeleteMatchStatusCommandHandler : IRequestHandler<DeleteMatchStatusCommand, Result<bool>>
    {
        private readonly IMatchStatusRepository _repository;

        public DeleteMatchStatusCommandHandler(IMatchStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteMatchStatusCommand request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<bool>.Failure("Match status not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete match status");
            }

            return Result<bool>.Success(true);
        }
    }
}
