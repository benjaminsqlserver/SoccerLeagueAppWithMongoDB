using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetAllSeasonStatuses
{
    public class GetAllSeasonStatusesQuery : IRequest<Result<PagedResult<SeasonStatusDto>>>
    {
        public QueryParameters Parameters { get; set; } = new QueryParameters();
    }
}
