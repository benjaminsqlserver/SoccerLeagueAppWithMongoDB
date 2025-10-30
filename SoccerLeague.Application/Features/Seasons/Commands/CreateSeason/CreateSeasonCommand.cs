using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Commands.CreateSeason
{
    public class CreateSeasonCommand : IRequest<Result<SeasonDto>>
    {
        public CreateSeasonDto Season { get; set; } = null!;
    }
}
