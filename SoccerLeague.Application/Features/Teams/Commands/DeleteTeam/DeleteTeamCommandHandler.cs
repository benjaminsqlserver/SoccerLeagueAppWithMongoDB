using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Teams.Commands.DeleteTeam
{
    public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Result<bool>>
    {
        private readonly ITeamRepository _repository;

        public DeleteTeamCommandHandler(ITeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await _repository.GetByIdAsync(request.Id);
            if (team == null)
            {
                return Result<bool>.Failure("Team not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete team");
            }

            return Result<bool>.Success(true);
        }
    }
}
