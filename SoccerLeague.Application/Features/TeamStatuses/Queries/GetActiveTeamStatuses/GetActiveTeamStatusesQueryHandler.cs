using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetActiveTeamStatuses
{
    public class GetActiveTeamStatusesQueryHandler : IRequestHandler<GetActiveTeamStatusesQuery, Result<List<TeamStatusDto>>>
    {
        private readonly ITeamStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveTeamStatusesQueryHandler(ITeamStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<TeamStatusDto>>> Handle(GetActiveTeamStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _repository.GetActiveStatusesAsync();
            var statusDtos = _mapper.Map<List<TeamStatusDto>>(statuses);

            return Result<List<TeamStatusDto>>.Success(statusDtos);
        }
    }
}
