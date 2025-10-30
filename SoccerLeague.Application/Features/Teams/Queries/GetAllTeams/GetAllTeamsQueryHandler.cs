using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Queries.GetAllTeams
{
    public class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, Result<PagedResult<TeamDto>>>
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTeamsQueryHandler(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<TeamDto>>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            var pagedTeams = await _repository.GetTeamsAsync(request.Parameters);
            var teamDtos = _mapper.Map<List<TeamDto>>(pagedTeams.Items);

            var pagedResult = new PagedResult<TeamDto>
            {
                Items = teamDtos,
                PageNumber = pagedTeams.PageNumber,
                PageSize = pagedTeams.PageSize,
                TotalCount = pagedTeams.TotalCount
            };

            return Result<PagedResult<TeamDto>>.Success(pagedResult);
        }
    }
}
