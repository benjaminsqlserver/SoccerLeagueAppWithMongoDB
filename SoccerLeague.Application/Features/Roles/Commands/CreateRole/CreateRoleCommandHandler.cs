using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Role;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateRoleCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<RoleDto>.Failure(errors);
            }

            var role = _mapper.Map<Role>(request.Role);
            role.CreatedDate = DateTime.UtcNow;

            var createdRole = await _repository.AddAsync(role);
            var roleDto = _mapper.Map<RoleDto>(createdRole);

            return Result<RoleDto>.Success(roleDto);
        }
    }
}
