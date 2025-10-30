using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Queries.GetAllStandings
{
    public class GetAllStandingsQueryHandler : IRequestHandler<GetAllStandingsQuery, Result<PagedResult<StandingDto>>>
    {
        private readonly IStandingRepository _repository;
        private readonly IMapper _mapper;

        public GetAllStandingsQueryHandler(IStandingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<StandingDto>>> Handle(GetAllStandingsQuery request, CancellationToken cancellationToken)
        {
            var pagedStandings = await _repository.GetStandingsAsync(request.Parameters);
            var standingDtos = _mapper.Map<List<StandingDto>>(pagedStandings.Items);

            var pagedResult = new PagedResult<StandingDto>
            {
                Items = standingDtos,
                PageNumber = pagedStandings.PageNumber,
                PageSize = pagedStandings.PageSize,
                TotalCount = pagedStandings.TotalCount
            };

            return Result<PagedResult<StandingDto>>.Success(pagedResult);
        }
    }
}
