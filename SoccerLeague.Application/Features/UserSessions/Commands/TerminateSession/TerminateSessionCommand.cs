using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Commands.TerminateSession
{
    public class TerminateSessionCommand : IRequest<Result<bool>>
    {
        public TerminateSessionDto TerminateData { get; set; } = null!;
    }
}
