using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.CreateTeamStatus
{
    public class CreateTeamStatusCommand : IRequest<Result<TeamStatusDto>>
    {
        public CreateTeamStatusDto TeamStatus { get; set; } = null!;
    }
}