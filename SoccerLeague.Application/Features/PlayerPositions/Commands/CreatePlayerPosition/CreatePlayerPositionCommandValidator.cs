using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.CreatePlayerPosition
{
    public class CreatePlayerPositionCommandValidator : AbstractValidator<CreatePlayerPositionCommand>
    {
        private readonly IPlayerPositionRepository _repository;

        public CreatePlayerPositionCommandValidator(IPlayerPositionRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.PlayerPosition.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(x => x.PlayerPosition.Code)
                .NotEmpty().WithMessage("Code is required")
                .MaximumLength(10).WithMessage("Code cannot exceed 10 characters")
                .MustAsync(BeUniqueCode).WithMessage("Code already exists");

            RuleFor(x => x.PlayerPosition.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.PlayerPosition.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            return !await _repository.CodeExistsAsync(code);
        }
    }
}
