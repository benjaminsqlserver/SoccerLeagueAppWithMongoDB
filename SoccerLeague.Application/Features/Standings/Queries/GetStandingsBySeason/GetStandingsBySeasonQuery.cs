using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Standing;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Standings.Queries.GetStandingsBySeason
{
    public class GetStandingsBySeasonQuery : IRequest<Result<List<StandingDto>>>
    {
        public string SeasonId { get; set; } = string.Empty;
    }
}
