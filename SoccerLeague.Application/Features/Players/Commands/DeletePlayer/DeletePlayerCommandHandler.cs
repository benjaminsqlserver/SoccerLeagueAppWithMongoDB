using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Players.Commands.DeletePlayer
{
    public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, Result<bool>>
    {
        private readonly IPlayerRepository _repository;

        public DeletePlayerCommandHandler(IPlayerRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
        {
            var player = await _repository.GetByIdAsync(request.Id);
            if (player == null)
            {
                return Result<bool>.Failure("Player not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete player");
            }

            return Result<bool>.Success(true);
        }
    }
}
