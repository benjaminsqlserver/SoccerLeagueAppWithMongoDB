using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetUserSessions
{
    public class GetUserSessionsQuery : IRequest<Result<List<UserSessionDto>>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
