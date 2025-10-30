using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.DeleteTeamStatus
{
    public class DeleteTeamStatusCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
