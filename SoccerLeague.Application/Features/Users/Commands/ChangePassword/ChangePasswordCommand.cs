using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommand : IRequest<Result<bool>>
    {
        public ChangePasswordDto PasswordData { get; set; } = null!;
    }
}
