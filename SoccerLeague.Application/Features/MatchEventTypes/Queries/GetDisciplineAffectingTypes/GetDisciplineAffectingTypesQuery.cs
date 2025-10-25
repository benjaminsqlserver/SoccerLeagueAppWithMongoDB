using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetDisciplineAffectingTypes
{
    public class GetDisciplineAffectingTypesQuery : IRequest<Result<List<MatchEventTypeDto>>>
    {
    }
}