using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Queries.GetTeamById
{
    public class GetTeamByIdQuery : IRequest<Result<TeamDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
