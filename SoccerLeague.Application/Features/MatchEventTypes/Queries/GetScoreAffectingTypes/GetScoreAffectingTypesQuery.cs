using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetScoreAffectingTypes
{
    public class GetScoreAffectingTypesQuery : IRequest<Result<List<MatchEventTypeDto>>>
    {
    }
}
