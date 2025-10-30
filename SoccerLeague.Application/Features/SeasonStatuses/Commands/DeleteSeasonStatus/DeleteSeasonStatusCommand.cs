using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.DeleteSeasonStatus
{
    public class DeleteSeasonStatusCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
