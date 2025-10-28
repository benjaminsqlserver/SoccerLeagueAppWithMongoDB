using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.UpdateMatchStatus
{
    public class UpdateMatchStatusCommandValidator : AbstractValidator<UpdateMatchStatusCommand>
    {
        private readonly IMatchStatusRepository _repository;

        public UpdateMatchStatusCommandValidator(IMatchStatusRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.MatchStatus.Id)
                .NotEmpty().WithMessage("Id is required")
                .MustAsync(StatusExists).WithMessage("Match status does not exist");

            RuleFor(x => x.MatchStatus.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.MatchStatus.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot exceed 10 characters")
                .MustAsync(BeUniqueCode).WithMessage("Code already exists");

            RuleFor(x => x.MatchStatus.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.MatchStatus.ColorCode)
                .MaximumLength(20).WithMessage("Color code cannot exceed 20 characters")
                .Matches("^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.MatchStatus.ColorCode))
                .WithMessage("Color code must be a valid hex color (e.g., #22c55e)");

            RuleFor(x => x.MatchStatus.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");
        }

        private async Task<bool> StatusExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueCode(UpdateMatchStatusCommand command, string code, CancellationToken cancellationToken)
        {
            return !await _repository.CodeExistsAsync(code, command.MatchStatus.Id);
        }
    }
}
