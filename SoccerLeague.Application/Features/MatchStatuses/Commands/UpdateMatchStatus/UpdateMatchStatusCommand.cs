using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.UpdateMatchStatus
{
    public class UpdateMatchStatusCommand : IRequest<Result<MatchStatusDto>>
    {
        public UpdateMatchStatusDto MatchStatus { get; set; } = null!;
    }
}
