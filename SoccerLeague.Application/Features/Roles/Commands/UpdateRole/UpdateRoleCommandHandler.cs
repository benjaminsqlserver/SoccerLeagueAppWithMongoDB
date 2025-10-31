using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public UpdateRoleCommandHandler(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateRoleCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<RoleDto>.Failure(errors);
            }

            var existingRole = await _repository.GetByIdAsync(request.Role.Id);
            if (existingRole == null)
            {
                return Result<RoleDto>.Failure("Role not found");
            }

            // Prevent modification of system role names
            if (existingRole.IsSystemRole && existingRole.Name != request.Role.Name)
            {
                return Result<RoleDto>.Failure("Cannot change the name of a system role");
            }

            _mapper.Map(request.Role, existingRole);
            existingRole.ModifiedDate = DateTime.UtcNow;

            var updatedRole = await _repository.UpdateAsync(existingRole);
            var roleDto = _mapper.Map<RoleDto>(updatedRole);

            return Result<RoleDto>.Success(roleDto);
        }
    }
}
