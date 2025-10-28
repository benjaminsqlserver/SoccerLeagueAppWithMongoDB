using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.CreateMatchStatus
{
    public class CreateMatchStatusCommand : IRequest<Result<MatchStatusDto>>
    {
        public CreateMatchStatusDto MatchStatus { get; set; } = null!;
    }
}
