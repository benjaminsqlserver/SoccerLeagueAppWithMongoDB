using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetAllMatchEventTypes
{
    public class GetAllMatchEventTypesQuery : IRequest<Result<PagedResult<MatchEventTypeDto>>>
    {
        public QueryParameters Parameters { get; set; } = new QueryParameters();
    }
}
