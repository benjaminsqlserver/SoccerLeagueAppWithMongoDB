using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchesBySeason
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchesBySeasonQueryHandler : IRequestHandler<GetMatchesBySeasonQuery, Result<List<MatchDto>>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetMatchesBySeasonQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<MatchDto>>> Handle(GetMatchesBySeasonQuery request, CancellationToken cancellationToken)
        {
            var matches = await _matchRepository.GetMatchesBySeasonAsync(request.SeasonId);
            var matchDtos = _mapper.Map<List<MatchDto>>(matches);
            return Result<List<MatchDto>>.Success(matchDtos);
        }
    }
}

