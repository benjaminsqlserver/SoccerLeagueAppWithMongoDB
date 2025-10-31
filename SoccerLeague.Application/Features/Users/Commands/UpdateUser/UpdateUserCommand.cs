using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public UpdateUserDto User { get; set; } = null!;
    }
}
