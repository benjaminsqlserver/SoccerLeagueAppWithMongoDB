using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.UserSessions.Commands.TerminateAllUserSessions
{
    public class TerminateAllUserSessionsCommand : IRequest<Result<bool>>
    {
        public string UserId { get; set; } = string.Empty;
        public string TerminationReason { get; set; } = string.Empty;
    }
}
