using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.UpdateMatch
{
    using FluentValidation;
    using SoccerLeague.Application.Contracts.Persistence;

    public class UpdateMatchCommandValidator : AbstractValidator<UpdateMatchCommand>
    {
        private readonly IMatchRepository _matchRepository;

        public UpdateMatchCommandValidator(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;

            RuleFor(x => x.Match.Id)
                .NotEmpty().WithMessage("Match ID is required")
                .MustAsync(MatchExists).WithMessage("Match does not exist");

            RuleFor(x => x.Match.SeasonId)
                .NotEmpty().WithMessage("Season is required");

            RuleFor(x => x.Match.HomeTeamId)
                .NotEmpty().WithMessage("Home team is required");

            RuleFor(x => x.Match.AwayTeamId)
                .NotEmpty().WithMessage("Away team is required");

            RuleFor(x => x.Match)
                .Must(m => m.HomeTeamId != m.AwayTeamId)
                .WithMessage("Home team and away team must be different");

            RuleFor(x => x.Match.Venue)
                .NotEmpty().WithMessage("Venue is required")
                .MaximumLength(200).WithMessage("Venue cannot exceed 200 characters");

            RuleFor(x => x.Match.Round)
                .GreaterThan(0).WithMessage("Round must be greater than 0");

            RuleFor(x => x.Match.MatchStatusId)
                .NotEmpty().WithMessage("Match status is required");

            When(x => x.Match.HomeTeamScore.HasValue && x.Match.AwayTeamScore.HasValue, () =>
            {
                RuleFor(x => x.Match.HomeTeamScore)
                    .GreaterThanOrEqualTo(0).WithMessage("Home team score cannot be negative");

                RuleFor(x => x.Match.AwayTeamScore)
                    .GreaterThanOrEqualTo(0).WithMessage("Away team score cannot be negative");
            });
        }

        private async Task<bool> MatchExists(string id, CancellationToken cancellationToken)
        {
            return await _matchRepository.ExistsAsync(id);
        }
    }
}

