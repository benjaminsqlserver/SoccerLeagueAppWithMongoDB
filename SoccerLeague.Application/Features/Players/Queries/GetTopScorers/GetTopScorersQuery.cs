using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Players.Queries.GetTopScorers
{
    public class GetTopScorersQuery : IRequest<Result<List<PlayerDto>>>
    {
        public int Count { get; set; } = 10;
    }
}
