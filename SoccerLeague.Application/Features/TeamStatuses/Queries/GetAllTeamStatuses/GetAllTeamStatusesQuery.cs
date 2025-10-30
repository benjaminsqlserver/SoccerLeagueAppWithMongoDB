using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetAllTeamStatuses
{
    public class GetAllTeamStatusesQuery : IRequest<Result<PagedResult<TeamStatusDto>>>
    {
        public QueryParameters Parameters { get; set; } = new QueryParameters();
    }
}
