using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<Result<PagedResult<UserDto>>>
    {
        public UserQueryParameters Parameters { get; set; } = new UserQueryParameters();
    }
}
