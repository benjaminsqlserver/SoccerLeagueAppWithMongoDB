using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetRecentMatches
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class GetRecentMatchesQueryHandler : IRequestHandler<GetRecentMatchesQuery, Result<List<MatchDto>>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetRecentMatchesQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<MatchDto>>> Handle(GetRecentMatchesQuery request, CancellationToken cancellationToken)
        {
            var matches = await _matchRepository.GetRecentMatchesAsync(request.Count);
            var matchDtos = _mapper.Map<List<MatchDto>>(matches);
            return Result<List<MatchDto>>.Success(matchDtos);
        }
    }
}
