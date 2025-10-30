using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.Seasons.Commands.DeleteSeason
{
    public class DeleteSeasonCommand : IRequest<Result<bool>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
