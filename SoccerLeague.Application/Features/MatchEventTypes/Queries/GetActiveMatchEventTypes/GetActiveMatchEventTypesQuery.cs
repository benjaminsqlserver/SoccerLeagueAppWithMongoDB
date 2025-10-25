using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetActiveMatchEventTypes
{
    public class GetActiveMatchEventTypesQuery : IRequest<Result<List<MatchEventTypeDto>>>
    {
    }
}
