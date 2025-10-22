using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchesBySeason
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchesBySeasonQuery : IRequest<Result<List<MatchDto>>>
    {
        public string SeasonId { get; set; } = string.Empty;
    }
}
