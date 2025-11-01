using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetAllSessions
{
    public class GetAllSessionsQuery : IRequest<Result<PagedResult<UserSessionDto>>>
    {
        public UserSessionQueryParameters Parameters { get; set; } = new UserSessionQueryParameters();
    }
}
