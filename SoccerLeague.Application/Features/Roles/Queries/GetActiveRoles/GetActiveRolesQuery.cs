using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Roles.Queries.GetActiveRoles
{
    public class GetActiveRolesQuery : IRequest<Result<List<RoleDto>>>
    {
    }
}
