using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetAllPlayerPositions
{
    public class GetAllPlayerPositionsQuery : IRequest<Result<PagedResult<PlayerPositionDto>>>
    {
        public QueryParameters Parameters { get; set; } = new QueryParameters();
    }
}
