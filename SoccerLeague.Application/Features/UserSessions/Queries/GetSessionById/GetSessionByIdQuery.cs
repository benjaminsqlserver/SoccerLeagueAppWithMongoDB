using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetSessionById
{
    public class GetSessionByIdQuery : IRequest<Result<UserSessionDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
