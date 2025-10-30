using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetAllPlayers
{
    public class GetAllPlayersQuery : IRequest<Result<PagedResult<PlayerDto>>>
    {
        public PlayerQueryParameters Parameters { get; set; } = new PlayerQueryParameters();
    }
}
