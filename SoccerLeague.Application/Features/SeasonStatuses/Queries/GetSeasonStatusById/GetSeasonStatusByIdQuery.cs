using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetSeasonStatusById
{
    public class GetSeasonStatusByIdQuery : IRequest<Result<SeasonStatusDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
