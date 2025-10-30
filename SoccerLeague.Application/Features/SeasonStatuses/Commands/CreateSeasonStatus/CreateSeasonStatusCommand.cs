using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.CreateSeasonStatus
{
    public class CreateSeasonStatusCommand : IRequest<Result<SeasonStatusDto>>
    {
        public CreateSeasonStatusDto SeasonStatus { get; set; } = null!;
    }
}
