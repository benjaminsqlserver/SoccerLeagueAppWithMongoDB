using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetAllMatchStatuses
{
    public class GetAllMatchStatusesQueryHandler : IRequestHandler<GetAllMatchStatusesQuery, Result<PagedResult<MatchStatusDto>>>
    {
        private readonly IMatchStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMatchStatusesQueryHandler(IMatchStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<MatchStatusDto>>> Handle(GetAllMatchStatusesQuery request, CancellationToken cancellationToken)
        {
            var pagedStatuses = await _repository.GetPagedAsync(request.Parameters);
            var statusDtos = _mapper.Map<List<MatchStatusDto>>(pagedStatuses.Items);

            var pagedResult = new PagedResult<MatchStatusDto>
            {
                Items = statusDtos,
                PageNumber = pagedStatuses.PageNumber,
                PageSize = pagedStatuses.PageSize,
                TotalCount = pagedStatuses.TotalCount
            };

            return Result<PagedResult<MatchStatusDto>>.Success(pagedResult);
        }
    }
}
