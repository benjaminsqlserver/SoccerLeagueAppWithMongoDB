using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
