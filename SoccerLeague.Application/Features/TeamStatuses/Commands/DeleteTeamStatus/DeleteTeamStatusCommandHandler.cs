using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.DeleteTeamStatus
{
    public class DeleteTeamStatusCommandHandler : IRequestHandler<DeleteTeamStatusCommand, Result<bool>>
    {
        private readonly ITeamStatusRepository _repository;

        public DeleteTeamStatusCommandHandler(ITeamStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteTeamStatusCommand request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<bool>.Failure("Team status not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete team status");
            }

            return Result<bool>.Success(true);
        }
    }
}
