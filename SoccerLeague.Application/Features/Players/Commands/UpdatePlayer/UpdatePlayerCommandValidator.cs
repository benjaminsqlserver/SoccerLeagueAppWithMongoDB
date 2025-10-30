using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Players.Commands.UpdatePlayer
{
    public class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
    {
        private readonly IPlayerRepository _repository;

        public UpdatePlayerCommandValidator(IPlayerRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Player.Id)
                .NotEmpty().WithMessage("Player ID is required")
                .MustAsync(PlayerExists).WithMessage("Player does not exist");

            RuleFor(x => x.Player.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.Player.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Player.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past");

            RuleFor(x => x.Player.Nationality)
                .NotEmpty().WithMessage("Nationality is required")
                .MaximumLength(100).WithMessage("Nationality cannot exceed 100 characters");

            RuleFor(x => x.Player.JerseyNumber)
                .GreaterThan(0).WithMessage("Jersey number must be greater than 0")
                .LessThanOrEqualTo(99).WithMessage("Jersey number must not exceed 99")
                .MustAsync(BeUniqueJerseyNumberInTeam).WithMessage("Jersey number already exists in this team");

            RuleFor(x => x.Player.PlayerPositionId)
                .NotEmpty().WithMessage("Player position is required");

            RuleFor(x => x.Player.TeamId)
                .NotEmpty().WithMessage("Team is required");

            RuleFor(x => x.Player.Height)
                .GreaterThan(0).WithMessage("Height must be greater than 0");

            RuleFor(x => x.Player.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0");

            RuleFor(x => x.Player.PreferredFoot)
                .Must(foot => string.IsNullOrEmpty(foot) || foot == "Left" || foot == "Right" || foot == "Both")
                .WithMessage("Preferred foot must be 'Left', 'Right', or 'Both'");

            RuleFor(x => x.Player.Appearances)
                .GreaterThanOrEqualTo(0).WithMessage("Appearances cannot be negative");

            RuleFor(x => x.Player.Goals)
                .GreaterThanOrEqualTo(0).WithMessage("Goals cannot be negative");

            RuleFor(x => x.Player.Assists)
                .GreaterThanOrEqualTo(0).WithMessage("Assists cannot be negative");

            RuleFor(x => x.Player.YellowCards)
                .GreaterThanOrEqualTo(0).WithMessage("Yellow cards cannot be negative");

            RuleFor(x => x.Player.RedCards)
                .GreaterThanOrEqualTo(0).WithMessage("Red cards cannot be negative");

            RuleFor(x => x.Player.MinutesPlayed)
                .GreaterThanOrEqualTo(0).WithMessage("Minutes played cannot be negative");
        }

        private async Task<bool> PlayerExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueJerseyNumberInTeam(UpdatePlayerCommand command, int jerseyNumber, CancellationToken cancellationToken)
        {
            return !await _repository.JerseyNumberExistsInTeamAsync(command.Player.TeamId, jerseyNumber, command.Player.Id);
        }
    }
}
