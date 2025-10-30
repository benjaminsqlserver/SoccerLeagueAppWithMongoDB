using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetAllSeasons
{
    public class GetAllSeasonsQuery : IRequest<Result<PagedResult<SeasonDto>>>
    {
        public SeasonQueryParameters Parameters { get; set; } = new SeasonQueryParameters();
    }
}
