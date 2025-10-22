using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchesByTeam
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchesByTeamQuery : IRequest<Result<List<MatchDto>>>
    {
        public string TeamId { get; set; } = string.Empty;
    }
}

