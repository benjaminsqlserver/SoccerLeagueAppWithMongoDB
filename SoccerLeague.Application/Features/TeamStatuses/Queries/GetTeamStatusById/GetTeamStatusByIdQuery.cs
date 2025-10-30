using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetTeamStatusById
{
    public class GetTeamStatusByIdQuery : IRequest<Result<TeamStatusDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
