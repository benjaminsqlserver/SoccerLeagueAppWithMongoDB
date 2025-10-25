using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.CreateMatchEventType
{
    public class CreateMatchEventTypeCommand : IRequest<Result<MatchEventTypeDto>>
    {
        public CreateMatchEventTypeDto MatchEventType { get; set; } = null!;
    }
}