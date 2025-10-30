using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Teams.Commands.DeleteTeam
{
    public class DeleteTeamCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
