using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Queries.GetAllTeams
{
    public class GetAllTeamsQuery : IRequest<Result<PagedResult<TeamDto>>>
    {
        public TeamQueryParameters Parameters { get; set; } = new TeamQueryParameters();
    }
}
