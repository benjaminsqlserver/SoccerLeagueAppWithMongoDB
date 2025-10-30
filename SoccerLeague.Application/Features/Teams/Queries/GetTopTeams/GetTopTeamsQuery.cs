using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.Team;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.Teams.Queries.GetTopTeams
{
    public class GetTopTeamsQuery : IRequest<Result<List<TeamDto>>>
    {
        public int Count { get; set; } = 10;
    }
}
