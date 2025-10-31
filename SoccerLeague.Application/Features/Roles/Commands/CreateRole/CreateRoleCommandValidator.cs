using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        private readonly IRoleRepository _repository;

        public CreateRoleCommandValidator(IRoleRepository repository)
        {
            _repository = repository;

            RuleFor(x => x.Role.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters")
                .MustAsync(BeUniqueRoleName).WithMessage("Role name already exists");

            RuleFor(x => x.Role.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Role.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0");

            RuleFor(x => x.Role.Permissions)
                .NotNull().WithMessage("Permissions list cannot be null");
        }

        private async Task<bool> BeUniqueRoleName(string name, CancellationToken cancellationToken)
        {
            return !await _repository.RoleNameExistsAsync(name);
        }
    }
}
