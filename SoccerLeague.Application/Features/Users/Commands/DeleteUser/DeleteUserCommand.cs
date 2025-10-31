using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
