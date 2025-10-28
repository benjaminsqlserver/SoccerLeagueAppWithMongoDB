using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetAllMatchStatuses
{
    public class GetAllMatchStatusesQuery : IRequest<Result<PagedResult<MatchStatusDto>>>
    {
        public QueryParameters Parameters { get; set; } = new QueryParameters();
    }
}
