using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Queries.GetActiveRoles
{
    public class GetActiveRolesQueryHandler : IRequestHandler<GetActiveRolesQuery, Result<List<RoleDto>>>
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveRolesQueryHandler(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<RoleDto>>> Handle(GetActiveRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _repository.GetActiveRolesAsync();
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);

            return Result<List<RoleDto>>.Success(roleDtos);
        }
    }
}
