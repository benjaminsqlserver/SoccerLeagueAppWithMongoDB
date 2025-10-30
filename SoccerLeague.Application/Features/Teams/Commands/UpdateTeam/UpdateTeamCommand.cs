using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Commands.UpdateTeam
{
    public class UpdateTeamCommand : IRequest<Result<TeamDto>>
    {
        public UpdateTeamDto Team { get; set; } = null!;
    }
}
