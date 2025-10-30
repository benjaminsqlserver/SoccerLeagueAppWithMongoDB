using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.UpdateSeasonStatus
{
    public class UpdateSeasonStatusCommand : IRequest<Result<SeasonStatusDto>>
    {
        public UpdateSeasonStatusDto SeasonStatus { get; set; } = null!;
    }
}
