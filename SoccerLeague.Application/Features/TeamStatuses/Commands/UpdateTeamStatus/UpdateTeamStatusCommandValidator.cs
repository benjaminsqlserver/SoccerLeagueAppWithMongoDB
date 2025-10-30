using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.UpdateTeamStatus
{
    public class UpdateTeamStatusCommandValidator : AbstractValidator<UpdateTeamStatusCommand>
    {
        private readonly ITeamStatusRepository _repository;

        public UpdateTeamStatusCommandValidator(ITeamStatusRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.TeamStatus.Id)
                .NotEmpty().WithMessage("Id is required")
                .MustAsync(StatusExists).WithMessage("Team status does not exist");

            RuleFor(x => x.TeamStatus.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.TeamStatus.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot exceed 10 characters")
                .MustAsync(BeUniqueCode).WithMessage("Code already exists");

            RuleFor(x => x.TeamStatus.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.TeamStatus.ColorCode)
                .MaximumLength(20).WithMessage("Color code cannot exceed 20 characters")
                .Matches("^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.TeamStatus.ColorCode))
                .WithMessage("Color code must be a valid hex color (e.g., #22c55e)");

            RuleFor(x => x.TeamStatus.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");
        }

        private async Task<bool> StatusExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueCode(UpdateTeamStatusCommand command, string code, CancellationToken cancellationToken)
        {
            return !await _repository.CodeExistsAsync(code, command.TeamStatus.Id);
        }
    }
}
