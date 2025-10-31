using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest<Result<UserDto>>
    {
        public CreateUserDto User { get; set; } = null!;
    }
}
