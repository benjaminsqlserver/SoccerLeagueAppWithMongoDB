using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetPlayerPositionById
{
    public class GetPlayerPositionByIdQuery : IRequest<Result<PlayerPositionDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
