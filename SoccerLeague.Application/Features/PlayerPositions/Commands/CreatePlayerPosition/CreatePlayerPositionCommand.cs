using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.CreatePlayerPosition
{
    public class CreatePlayerPositionCommand : IRequest<Result<PlayerPositionDto>>
    {
        public CreatePlayerPositionDto PlayerPosition { get; set; } = null!;
    }
}
