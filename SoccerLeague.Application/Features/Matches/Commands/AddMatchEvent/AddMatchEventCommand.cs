using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.AddMatchEvent
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.DTOs.Match;

    public class AddMatchEventCommand : IRequest<Result<bool>>
    {
        public CreateMatchEventDto MatchEvent { get; set; } = null!;
    }
}
