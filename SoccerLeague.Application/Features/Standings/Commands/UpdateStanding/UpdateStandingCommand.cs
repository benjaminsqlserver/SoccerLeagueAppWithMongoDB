using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Commands.UpdateStanding
{
    public class UpdateStandingCommand : IRequest<Result<StandingDto>>
    {
        public UpdateStandingDto Standing { get; set; } = null!;
    }
}
