using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.UpdateMatchEventType
{
    public class UpdateMatchEventTypeCommand : IRequest<Result<MatchEventTypeDto>>
    {
        public UpdateMatchEventTypeDto MatchEventType { get; set; } = null!;
    }
}
