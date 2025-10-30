using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Players.Queries.GetPlayersByTeam
{
    public class GetPlayersByTeamQuery : IRequest<Result<List<PlayerDto>>>
    {
        public string TeamId { get; set; } = string.Empty;
    }
}
