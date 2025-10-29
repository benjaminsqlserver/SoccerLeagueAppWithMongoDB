using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.DeletePlayerPosition
{
    public class DeletePlayerPositionCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
