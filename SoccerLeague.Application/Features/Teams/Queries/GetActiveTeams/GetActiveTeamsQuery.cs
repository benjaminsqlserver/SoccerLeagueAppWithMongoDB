using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Teams.Queries.GetActiveTeams
{
    public class GetActiveTeamsQuery : IRequest<Result<List<TeamDto>>>
    {
    }
}
