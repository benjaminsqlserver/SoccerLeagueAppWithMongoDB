using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<PagedResult<RoleDto>>>
    {
        private readonly IRoleRepository _repository;
        private readonly IMapper _mapper;

        public GetAllRolesQueryHandler(IRoleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var pagedRoles = await _repository.GetRolesAsync(request.Parameters);
            var roleDtos = _mapper.Map<List<RoleDto>>(pagedRoles.Items);

            var pagedResult = new PagedResult<RoleDto>
            {
                Items = roleDtos,
                PageNumber = pagedRoles.PageNumber,
                PageSize = pagedRoles.PageSize,
                TotalCount = pagedRoles.TotalCount
            };

            return Result<PagedResult<RoleDto>>.Success(pagedResult);
        }
    }
}
