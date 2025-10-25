using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.DeleteMatchEventType
{
    public class DeleteMatchEventTypeCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}