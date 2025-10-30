using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Players.Commands.CreatePlayer
{
    public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        private readonly IPlayerRepository _repository;

        public CreatePlayerCommandValidator(IPlayerRepository repository)
        {
            _repository = repository;

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
        }

        private async Task<bool> BeUniqueJerseyNumberInTeam(CreatePlayerCommand command, int jerseyNumber, CancellationToken cancellationToken)
        {
            return !await _repository.JerseyNumberExistsInTeamAsync(command.Player.TeamId, jerseyNumber);
        }
    }
}
