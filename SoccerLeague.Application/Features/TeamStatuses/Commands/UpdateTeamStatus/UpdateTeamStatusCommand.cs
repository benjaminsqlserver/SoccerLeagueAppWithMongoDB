using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.UpdateTeamStatus
{
    public class UpdateTeamStatusCommand : IRequest<Result<TeamStatusDto>>
    {
        public UpdateTeamStatusDto TeamStatus { get; set; } = null!;
    }
}
