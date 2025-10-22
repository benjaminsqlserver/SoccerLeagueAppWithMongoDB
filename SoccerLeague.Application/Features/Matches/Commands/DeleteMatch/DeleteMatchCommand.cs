using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.DeleteMatch
{
    using MediatR;
    using SoccerLeague.Application.Common.Models;

    public class DeleteMatchCommand : IRequest<Result<bool>>
    {
        public string MatchId { get; set; } = string.Empty;
    }
}