using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.CreateMatch
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class CreateMatchCommand : IRequest<Result<MatchDto>>
    {
        public CreateMatchDto Match { get; set; } = null!;
    }
}
