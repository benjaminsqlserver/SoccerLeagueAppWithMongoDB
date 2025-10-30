using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Standings.Commands.DeleteStanding
{
    public class DeleteStandingCommandHandler : IRequestHandler<DeleteStandingCommand, Result<bool>>
    {
        private readonly IStandingRepository _repository;

        public DeleteStandingCommandHandler(IStandingRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteStandingCommand request, CancellationToken cancellationToken)
        {
            var standing = await _repository.GetByIdAsync(request.Id);
            if (standing == null)
            {
                return Result<bool>.Failure("Standing not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete standing");
            }

            return Result<bool>.Success(true);
        }
    }
}
