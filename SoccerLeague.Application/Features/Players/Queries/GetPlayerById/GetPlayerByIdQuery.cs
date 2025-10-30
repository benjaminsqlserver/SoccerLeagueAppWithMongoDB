using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetPlayerById
{
    public class GetPlayerByIdQuery : IRequest<Result<PlayerDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
