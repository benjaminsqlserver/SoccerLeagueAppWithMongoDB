using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Standings.Commands.UpdateStanding
{
    public class UpdateStandingCommandValidator : AbstractValidator<UpdateStandingCommand>
    {
        private readonly IStandingRepository _repository;

        public UpdateStandingCommandValidator(IStandingRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Standing.Id)
                .NotEmpty().WithMessage("Standing ID is required")
                .MustAsync(StandingExists).WithMessage("Standing does not exist");

            RuleFor(x => x.Standing.SeasonId)
                .NotEmpty().WithMessage("Season is required");

            RuleFor(x => x.Standing.TeamId)
                .NotEmpty().WithMessage("Team is required")
                .MustAsync(BeUniqueTeamInSeason).WithMessage("Standing already exists for this team in the season");

            RuleFor(x => x.Standing.Position)
                .GreaterThan(0).WithMessage("Position must be greater than 0");

            RuleFor(x => x.Standing.MatchesPlayed)
                .GreaterThanOrEqualTo(0).WithMessage("Matches played cannot be negative");

            RuleFor(x => x.Standing.Wins)
                .GreaterThanOrEqualTo(0).WithMessage("Wins cannot be negative");

            RuleFor(x => x.Standing.Draws)
                .GreaterThanOrEqualTo(0).WithMessage("Draws cannot be negative");

            RuleFor(x => x.Standing.Losses)
                .GreaterThanOrEqualTo(0).WithMessage("Losses cannot be negative");

            RuleFor(x => x.Standing.GoalsFor)
                .GreaterThanOrEqualTo(0).WithMessage("Goals for cannot be negative");

            RuleFor(x => x.Standing.GoalsAgainst)
                .GreaterThanOrEqualTo(0).WithMessage("Goals against cannot be negative");

            RuleFor(x => x.Standing.Points)
                .GreaterThanOrEqualTo(0).WithMessage("Points cannot be negative");
        }

        private async Task<bool> StandingExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueTeamInSeason(UpdateStandingCommand command, string teamId, CancellationToken cancellationToken)
        {
            return !await _repository.StandingExistsForTeamInSeasonAsync(command.Standing.SeasonId, teamId, command.Standing.Id);
        }
    }
}
