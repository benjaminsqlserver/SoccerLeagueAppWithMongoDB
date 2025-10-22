using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchById
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchByIdQuery : IRequest<Result<MatchDto>>
    {
        public string MatchId { get; set; } = string.Empty;
    }
}
