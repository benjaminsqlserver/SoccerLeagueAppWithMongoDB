using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.DeleteMatch
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;

    public class DeleteMatchCommandHandler : IRequestHandler<DeleteMatchCommand, Result<bool>>
    {
        private readonly IMatchRepository _matchRepository;

        public DeleteMatchCommandHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<Result<bool>> Handle(DeleteMatchCommand request, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetByIdAsync(request.MatchId);
            if (match == null)
            {
                return Result<bool>.Failure("Match not found");
            }

            var result = await _matchRepository.DeleteAsync(request.MatchId);

            if (!result)
            {
                return Result<bool>.Failure("Failed to delete match");
            }

            return Result<bool>.Success(true);
        }
    }
}
