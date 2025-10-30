using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Standings.Commands.DeleteStanding
{
    public class DeleteStandingCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}