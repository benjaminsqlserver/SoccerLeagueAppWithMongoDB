using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.TeamStatus;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetActiveTeamStatuses
{
    public class GetActiveTeamStatusesQuery : IRequest<Result<List<TeamStatusDto>>>
    {
    }
}
