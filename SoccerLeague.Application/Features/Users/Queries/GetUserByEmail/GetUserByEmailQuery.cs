using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Queries.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<Result<UserDto>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
