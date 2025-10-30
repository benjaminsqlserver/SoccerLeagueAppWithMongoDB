using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Queries.GetStandingsBySeason
{
    public class GetStandingsBySeasonQueryHandler : IRequestHandler<GetStandingsBySeasonQuery, Result<List<StandingDto>>>
    {
        private readonly IStandingRepository _repository;
        private readonly IMapper _mapper;

        public GetStandingsBySeasonQueryHandler(IStandingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<StandingDto>>> Handle(GetStandingsBySeasonQuery request, CancellationToken cancellationToken)
        {
            var standings = await _repository.GetStandingsBySeasonAsync(request.SeasonId);
            var standingDtos = _mapper.Map<List<StandingDto>>(standings);

            return Result<List<StandingDto>>.Success(standingDtos);
        }
    }
}
