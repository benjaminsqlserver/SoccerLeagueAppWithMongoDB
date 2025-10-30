using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Queries.GetTopTeams
{
    public class GetTopTeamsQueryHandler : IRequestHandler<GetTopTeamsQuery, Result<List<TeamDto>>>
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public GetTopTeamsQueryHandler(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<TeamDto>>> Handle(GetTopTeamsQuery request, CancellationToken cancellationToken)
        {
            var teams = await _repository.GetTopTeamsByPointsAsync(request.Count);
            var teamDtos = _mapper.Map<List<TeamDto>>(teams);

            return Result<List<TeamDto>>.Success(teamDtos);
        }
    }
}
