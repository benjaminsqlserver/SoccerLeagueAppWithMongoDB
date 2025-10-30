using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetCurrentSeason
{
    public class GetCurrentSeasonQuery : IRequest<Result<SeasonDto>>
    {
    }
}
