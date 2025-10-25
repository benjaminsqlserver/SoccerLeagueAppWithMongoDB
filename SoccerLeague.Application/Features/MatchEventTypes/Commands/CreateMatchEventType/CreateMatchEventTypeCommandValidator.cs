using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.CreateMatchEventType
{
    public class CreateMatchEventTypeCommandValidator : AbstractValidator<CreateMatchEventTypeCommand>
    {
        private readonly IMatchEventTypeRepository _repository;

        public CreateMatchEventTypeCommandValidator(IMatchEventTypeRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.MatchEventType.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.MatchEventType.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot exceed 10 characters")
                .MustAsync(BeUniqueCode).WithMessage("Code already exists");

            RuleFor(x => x.MatchEventType.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.MatchEventType.IconName)
                .MaximumLength(50).WithMessage("Icon name cannot exceed 50 characters");

            RuleFor(x => x.MatchEventType.ColorCode)
                .MaximumLength(20).WithMessage("Color code cannot exceed 20 characters")
                .Matches("^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.MatchEventType.ColorCode))
                .WithMessage("Color code must be a valid hex color (e.g., #22c55e)");

            RuleFor(x => x.MatchEventType.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _repository.CodeExistsAsync(code);
        }
    }
}
