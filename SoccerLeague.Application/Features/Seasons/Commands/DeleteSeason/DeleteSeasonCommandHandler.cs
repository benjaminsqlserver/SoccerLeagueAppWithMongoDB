using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Seasons.Commands.DeleteSeason
{
    public class DeleteSeasonCommandHandler : IRequestHandler<DeleteSeasonCommand, Result<bool>>
    {
        private readonly ISeasonRepository _repository;

        public DeleteSeasonCommandHandler(ISeasonRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
        {
            var season = await _repository.GetByIdAsync(request.Id);
            if (season == null)
            {
                return Result<bool>.Failure("Season not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete season");
            }

            return Result<bool>.Success(true);
        }
    }
}
