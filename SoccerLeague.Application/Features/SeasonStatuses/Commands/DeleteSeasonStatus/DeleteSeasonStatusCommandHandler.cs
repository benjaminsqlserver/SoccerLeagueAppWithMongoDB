using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.DeleteSeasonStatus
{
    public class DeleteSeasonStatusCommandHandler : IRequestHandler<DeleteSeasonStatusCommand, Result<bool>>
    {
        private readonly ISeasonStatusRepository _repository;

        public DeleteSeasonStatusCommandHandler(ISeasonStatusRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteSeasonStatusCommand request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<bool>.Failure("Season status not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete season status");
            }

            return Result<bool>.Success(true);
        }
    }
}
