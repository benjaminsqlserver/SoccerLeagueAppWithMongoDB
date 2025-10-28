using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchStatus;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetActiveMatchStatuses
{
    public class GetActiveMatchStatusesQuery : IRequest<Result<List<MatchStatusDto>>>
    {
    }
}