using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetMatchStatusById
{
    public class GetMatchStatusByIdQuery : IRequest<Result<MatchStatusDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
