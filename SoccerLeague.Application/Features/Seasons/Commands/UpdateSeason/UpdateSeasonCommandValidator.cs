using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Seasons.Commands.UpdateSeason
{
    public class UpdateSeasonCommandValidator : AbstractValidator<UpdateSeasonCommand>
    {
        private readonly ISeasonRepository _repository;

        public UpdateSeasonCommandValidator(ISeasonRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Season.Id)
                .NotEmpty().WithMessage("Season ID is required")
                .MustAsync(SeasonExists).WithMessage("Season does not exist");

            RuleFor(x => x.Season.Name)
                .NotEmpty().WithMessage("Season name is required")
                .MaximumLength(100).WithMessage("Season name cannot exceed 100 characters")
                .MustAsync(BeUniqueName).WithMessage("Season name already exists");

            RuleFor(x => x.Season.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.Season.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThan(x => x.Season.StartDate).WithMessage("End date must be after start date");

            RuleFor(x => x.Season.SeasonStatusId)
                .NotEmpty().WithMessage("Season status is required");

            RuleFor(x => x.Season.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Season.NumberOfTeams)
                .GreaterThan(0).WithMessage("Number of teams must be greater than 0");

            RuleFor(x => x.Season.MatchesPerTeam)
                .GreaterThan(0).WithMessage("Matches per team must be greater than 0");

            RuleFor(x => x.Season.PointsForWin)
                .GreaterThanOrEqualTo(0).WithMessage("Points for win cannot be negative");

            RuleFor(x => x.Season.PointsForDraw)
                .GreaterThanOrEqualTo(0).WithMessage("Points for draw cannot be negative");

            RuleFor(x => x.Season.PointsForLoss)
                .GreaterThanOrEqualTo(0).WithMessage("Points for loss cannot be negative");
        }

        private async Task<bool> SeasonExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueName(UpdateSeasonCommand command, string name, CancellationToken cancellationToken)
        {
            return !await _repository.SeasonNameExistsAsync(name, command.Season.Id);
        }
    }
}
