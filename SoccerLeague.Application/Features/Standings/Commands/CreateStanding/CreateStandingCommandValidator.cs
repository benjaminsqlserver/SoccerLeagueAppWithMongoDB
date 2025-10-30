using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Standings.Commands.CreateStanding
{
    public class CreateStandingCommandValidator : AbstractValidator<CreateStandingCommand>
    {
        private readonly IStandingRepository _repository;

        public CreateStandingCommandValidator(IStandingRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Standing.SeasonId)
                .NotEmpty().WithMessage("Season is required");

            RuleFor(x => x.Standing.TeamId)
                .NotEmpty().WithMessage("Team is required")
                .MustAsync(BeUniqueTeamInSeason).WithMessage("Standing already exists for this team in the season");

            RuleFor(x => x.Standing.Position)
                .GreaterThan(0).WithMessage("Position must be greater than 0");
        }

        private async Task<bool> BeUniqueTeamInSeason(CreateStandingCommand command, string teamId, CancellationToken cancellationToken)
        {
            return !await _repository.StandingExistsForTeamInSeasonAsync(command.Standing.SeasonId, teamId);
        }
    }
}