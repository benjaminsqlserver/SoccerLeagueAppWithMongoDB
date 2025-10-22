using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.CreateMatch
{
    using FluentValidation;

    public class CreateMatchCommandValidator : AbstractValidator<CreateMatchCommand>
    {
        public CreateMatchCommandValidator()
        {
            RuleFor(x => x.Match.SeasonId)
                .NotEmpty().WithMessage("Season is required");

            RuleFor(x => x.Match.HomeTeamId)
                .NotEmpty().WithMessage("Home team is required");

            RuleFor(x => x.Match.AwayTeamId)
                .NotEmpty().WithMessage("Away team is required");

            RuleFor(x => x.Match)
                .Must(m => m.HomeTeamId != m.AwayTeamId)
                .WithMessage("Home team and away team must be different");

            RuleFor(x => x.Match.ScheduledDate)
                .NotEmpty().WithMessage("Scheduled date is required");

            RuleFor(x => x.Match.Venue)
                .NotEmpty().WithMessage("Venue is required")
                .MaximumLength(200).WithMessage("Venue cannot exceed 200 characters");

            RuleFor(x => x.Match.Round)
                .GreaterThan(0).WithMessage("Round must be greater than 0");

            RuleFor(x => x.Match.MatchStatusId)
                .NotEmpty().WithMessage("Match status is required");
        }
    }
}
