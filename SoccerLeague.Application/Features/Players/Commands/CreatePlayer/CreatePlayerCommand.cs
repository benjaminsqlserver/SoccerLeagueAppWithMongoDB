using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Commands.CreatePlayer
{
    public class CreatePlayerCommand : IRequest<Result<PlayerDto>>
    {
        public CreatePlayerDto Player { get; set; } = null!;
    }
}
