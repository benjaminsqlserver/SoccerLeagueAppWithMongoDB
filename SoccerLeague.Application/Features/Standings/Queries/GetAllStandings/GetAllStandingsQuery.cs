using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Queries.GetAllStandings
{
    public class GetAllStandingsQuery : IRequest<Result<PagedResult<StandingDto>>>
    {
        public StandingQueryParameters Parameters { get; set; } = new StandingQueryParameters();
    }
}
