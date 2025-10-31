using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Commands.UpdateRole
{
    public class UpdateRoleCommand : IRequest<Result<RoleDto>>
    {
        public UpdateRoleDto Role { get; set; } = null!;
    }
}
