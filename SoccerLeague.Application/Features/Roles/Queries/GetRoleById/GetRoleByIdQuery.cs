using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<Result<RoleDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
