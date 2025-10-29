using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.UpdatePlayerPosition
{
    public class UpdatePlayerPositionCommand : IRequest<Result<PlayerPositionDto>>
    {
        public UpdatePlayerPositionDto PlayerPosition { get; set; } = null!;
    }
}
