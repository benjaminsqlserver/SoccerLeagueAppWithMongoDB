using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<Result<PagedResult<RoleDto>>>
    {
        public RoleQueryParameters Parameters { get; set; } = new RoleQueryParameters();
    }
}
