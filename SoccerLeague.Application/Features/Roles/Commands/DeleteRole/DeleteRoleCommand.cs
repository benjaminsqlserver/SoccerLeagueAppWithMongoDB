using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
