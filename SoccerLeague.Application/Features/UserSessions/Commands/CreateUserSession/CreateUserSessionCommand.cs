using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Commands.CreateUserSession
{
    public class CreateUserSessionCommand : IRequest<Result<UserSessionDto>>
    {
        public CreateUserSessionDto Session { get; set; } = null!;
    }
}
