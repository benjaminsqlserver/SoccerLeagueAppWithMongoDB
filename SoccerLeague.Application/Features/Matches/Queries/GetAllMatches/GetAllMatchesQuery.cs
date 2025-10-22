using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetAllMatches
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class GetAllMatchesQuery : IRequest<Result<PagedResult<MatchDto>>>
    {
        public MatchQueryParameters Parameters { get; set; } = new MatchQueryParameters();
    }
}

