using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.DeleteMatchStatus
{
    public class DeleteMatchStatusCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
