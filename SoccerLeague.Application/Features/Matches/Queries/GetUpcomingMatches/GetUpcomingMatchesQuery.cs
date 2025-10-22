using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetUpcomingMatches
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class GetUpcomingMatchesQuery : IRequest<Result<List<MatchDto>>>
    {
        public int Count { get; set; } = 10;
    }
}
