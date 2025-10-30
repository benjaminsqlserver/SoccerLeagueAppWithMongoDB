using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetSeasonById
{
    public class GetSeasonByIdQuery : IRequest<Result<SeasonDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
