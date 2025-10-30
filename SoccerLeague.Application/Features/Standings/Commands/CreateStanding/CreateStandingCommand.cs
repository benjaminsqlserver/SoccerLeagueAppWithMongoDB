using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Commands.CreateStanding
{
    public class CreateStandingCommand : IRequest<Result<StandingDto>>
    {
        public CreateStandingDto Standing { get; set; } = null!;
    }
}
