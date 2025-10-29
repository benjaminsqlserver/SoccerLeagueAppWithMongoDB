using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.PlayerPosition;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetActivePlayerPositions
{
    public class GetActivePlayerPositionsQuery : IRequest<Result<List<PlayerPositionDto>>>
    {
    }
}
