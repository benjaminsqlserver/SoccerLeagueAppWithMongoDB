using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetMatchEventTypeById
{
    public class GetMatchEventTypeByIdQuery : IRequest<Result<MatchEventTypeDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
