using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.SeasonStatus;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetActiveSeasonStatuses
{
    public class GetActiveSeasonStatusesQuery : IRequest<Result<List<SeasonStatusDto>>>
    {
    }
}
