using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Users.Queries.GetActiveUsers
{
    public class GetActiveUsersQuery : IRequest<Result<List<UserDto>>>
    {
    }
}
