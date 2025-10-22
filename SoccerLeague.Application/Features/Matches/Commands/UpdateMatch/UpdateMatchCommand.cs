using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.UpdateMatch
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class UpdateMatchCommand : IRequest<Result<MatchDto>>
    {
        public UpdateMatchDto Match { get; set; } = null!;
    }
}