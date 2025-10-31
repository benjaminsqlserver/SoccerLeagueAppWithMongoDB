using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result<bool>>
    {
        private readonly IRoleRepository _repository;

        public DeleteRoleCommandHandler(IRoleRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _repository.GetByIdAsync(request.Id);
            if (role == null)
            {
                return Result<bool>.Failure("Role not found");
            }

            // Prevent deletion of system roles
            if (role.IsSystemRole)
            {
                return Result<bool>.Failure("Cannot delete a system role");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete role");
            }

            return Result<bool>.Success(true);
        }
    }
}
