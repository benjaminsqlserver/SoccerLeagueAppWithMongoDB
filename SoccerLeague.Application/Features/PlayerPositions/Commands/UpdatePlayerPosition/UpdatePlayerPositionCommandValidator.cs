using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.UpdatePlayerPosition
{
    public class UpdatePlayerPositionCommandValidator : AbstractValidator<UpdatePlayerPositionCommand>
    {
        private readonly IPlayerPositionRepository _repository;

        public UpdatePlayerPositionCommandValidator(IPlayerPositionRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.PlayerPosition.Id)
                .NotEmpty().WithMessage("Id is required")
                .MustAsync(PositionExists).WithMessage("Player position does not exist");

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

        private async Task<bool> PositionExists(string id, CancellationToken cancellationToken)
        {
            return await _repository.ExistsAsync(id);
        }

        private async Task<bool> BeUniqueCode(UpdatePlayerPositionCommand command, string code, CancellationToken cancellationToken)
        {
            return !await _repository.CodeExistsAsync(code, command.PlayerPosition.Id);
        }
    }
}
