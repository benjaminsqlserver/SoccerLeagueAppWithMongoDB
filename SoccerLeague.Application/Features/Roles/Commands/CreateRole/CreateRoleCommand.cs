using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Role;

namespace SoccerLeague.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<Result<RoleDto>>
    {
        public CreateRoleDto Role { get; set; } = null!;
    }
}
