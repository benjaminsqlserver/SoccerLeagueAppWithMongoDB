using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public GetRoleByIdQueryHandler(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _repository.GetByIdAsync(request.Id);
            if (role == null)
            {
                return Result<RoleDto>.Failure("Role not found");
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result<RoleDto>.Success(roleDto);
        }
    }
}
