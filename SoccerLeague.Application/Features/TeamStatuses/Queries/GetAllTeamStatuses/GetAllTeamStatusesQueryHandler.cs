using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetAllTeamStatuses
{
    public class GetAllTeamStatusesQueryHandler : IRequestHandler<GetAllTeamStatusesQuery, Result<PagedResult<TeamStatusDto>>>
    {
        private readonly ITeamStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTeamStatusesQueryHandler(ITeamStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<TeamStatusDto>>> Handle(GetAllTeamStatusesQuery request, CancellationToken cancellationToken)
        {
            var pagedStatuses = await _repository.GetPagedAsync(request.Parameters);
            var statusDtos = _mapper.Map<List<TeamStatusDto>>(pagedStatuses.Items);

            var pagedResult = new PagedResult<TeamStatusDto>
            {
                Items = statusDtos,
                PageNumber = pagedStatuses.PageNumber,
                PageSize = pagedStatuses.PageSize,
                TotalCount = pagedStatuses.TotalCount
            };

            return Result<PagedResult<TeamStatusDto>>.Success(pagedResult);
        }
    }
}
