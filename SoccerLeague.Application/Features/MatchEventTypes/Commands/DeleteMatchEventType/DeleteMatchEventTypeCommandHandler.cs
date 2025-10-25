using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.DeleteMatchEventType
{
    public class DeleteMatchEventTypeCommandHandler : IRequestHandler<DeleteMatchEventTypeCommand, Result<bool>>
    {
        private readonly IMatchEventTypeRepository _repository;

        public DeleteMatchEventTypeCommandHandler(IMatchEventTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteMatchEventTypeCommand request, CancellationToken cancellationToken)
        {
            var eventType = await _repository.GetByIdAsync(request.Id);
            if (eventType == null)
            {
                return Result<bool>.Failure("Match event type not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete match event type");
            }

            return Result<bool>.Success(true);
        }
    }
}
