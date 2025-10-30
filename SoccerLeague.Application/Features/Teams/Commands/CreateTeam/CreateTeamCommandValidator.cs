using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Teams.Commands.CreateTeam
{
    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        private readonly ITeamRepository _repository;

        public CreateTeamCommandValidator(ITeamRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Team.Name)
                .NotEmpty().WithMessage("Team name is required")
                .MaximumLength(200).WithMessage("Team name cannot exceed 200 characters")
                .MustAsync(BeUniqueName).WithMessage("Team name already exists");

            RuleFor(x => x.Team.ShortName)
                .NotEmpty().WithMessage("Short name is required")
                .MaximumLength(50).WithMessage("Short name cannot exceed 50 characters");

            RuleFor(x => x.Team.Stadium)
                .NotEmpty().WithMessage("Stadium is required")
                .MaximumLength(200).WithMessage("Stadium cannot exceed 200 characters");

            RuleFor(x => x.Team.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

            RuleFor(x => x.Team.Country)
                .NotEmpty().WithMessage("Country is required")
                .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

            RuleFor(x => x.Team.Manager)
                .NotEmpty().WithMessage("Manager is required")
                .MaximumLength(200).WithMessage("Manager cannot exceed 200 characters");

            RuleFor(x => x.Team.TeamStatusId)
                .NotEmpty().WithMessage("Team status is required");

            RuleFor(x => x.Team.FoundedDate)
                .NotEmpty().WithMessage("Founded date is required")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Founded date cannot be in the future");

            RuleFor(x => x.Team.ContactEmail)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Team.ContactEmail))
                .WithMessage("Invalid email format");

            RuleFor(x => x.Team.ContactPhone)
                .MaximumLength(20).When(x => !string.IsNullOrEmpty(x.Team.ContactPhone))
                .WithMessage("Contact phone cannot exceed 20 characters");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
        {
            return !await _repository.TeamNameExistsAsync(name);
        }
    }
}
