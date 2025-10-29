using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.DeletePlayerPosition
{
    public class DeletePlayerPositionCommandHandler : IRequestHandler<DeletePlayerPositionCommand, Result<bool>>
    {
        private readonly IPlayerPositionRepository _repository;

        public DeletePlayerPositionCommandHandler(IPlayerPositionRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeletePlayerPositionCommand request, CancellationToken cancellationToken)
        {
            var position = await _repository.GetByIdAsync(request.Id);
            if (position == null)
            {
                return Result<bool>.Failure("Player position not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete player position");
            }

            return Result<bool>.Success(true);
        }
    }
}
