using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Players.Commands.DeletePlayer
{
    public class DeletePlayerCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
