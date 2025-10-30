using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Commands.CreateTeam
{
    public class CreateTeamCommand : IRequest<Result<TeamDto>>
    {
        public CreateTeamDto Team { get; set; } = null!;
    }
}
