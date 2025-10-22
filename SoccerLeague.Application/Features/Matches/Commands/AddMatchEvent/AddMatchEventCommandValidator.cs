using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.AddMatchEvent
{
    using FluentValidation;
    using SoccerLeague.Application.Contracts.Persistence;

    public class AddMatchEventCommandValidator : AbstractValidator<AddMatchEventCommand>
    {
        private readonly IMatchRepository _matchRepository;

        public AddMatchEventCommandValidator(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;

            RuleFor(x => x.MatchEvent.MatchId)
                .NotEmpty().WithMessage("Match ID is required")
                .MustAsync(MatchExists).WithMessage("Match does not exist");

            RuleFor(x => x.MatchEvent.MatchEventTypeId)
                .NotEmpty().WithMessage("Match event type is required");

            RuleFor(x => x.MatchEvent.Minute)
                .GreaterThanOrEqualTo(0).WithMessage("Minute cannot be negative")
                .LessThanOrEqualTo(120).WithMessage("Minute cannot exceed 120");

            RuleFor(x => x.MatchEvent.PlayerId)
                .NotEmpty().WithMessage("Player ID is required");

            RuleFor(x => x.MatchEvent.PlayerName)
                .NotEmpty().WithMessage("Player name is required");

            RuleFor(x => x.MatchEvent.TeamId)
                .NotEmpty().WithMessage("Team ID is required");
        }

        private async Task<bool> MatchExists(string matchId, CancellationToken cancellationToken)
        {
            return await _matchRepository.ExistsAsync(matchId);
        }
    }
}
