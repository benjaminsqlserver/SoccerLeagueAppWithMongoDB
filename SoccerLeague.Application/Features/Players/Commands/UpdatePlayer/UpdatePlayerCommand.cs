using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Commands.UpdatePlayer
{
    public class UpdatePlayerCommand : IRequest<Result<PlayerDto>>
    {
        public UpdatePlayerDto Player { get; set; } = null!;
    }
}
