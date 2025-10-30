using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Commands.UpdateSeason
{
    public class UpdateSeasonCommand : IRequest<Result<SeasonDto>>
    {
        public UpdateSeasonDto Season { get; set; } = null!;
    }
}
