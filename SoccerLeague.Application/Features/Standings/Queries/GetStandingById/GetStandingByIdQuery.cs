using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Queries.GetStandingById
{
    public class GetStandingByIdQuery : IRequest<Result<StandingDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
