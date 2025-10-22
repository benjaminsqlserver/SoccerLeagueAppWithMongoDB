using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchesByTeam
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchesByTeamQueryHandler : IRequestHandler<GetMatchesByTeamQuery, Result<List<MatchDto>>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetMatchesByTeamQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<MatchDto>>> Handle(GetMatchesByTeamQuery request, CancellationToken cancellationToken)
        {
            var matches = await _matchRepository.GetMatchesByTeamAsync(request.TeamId);
            var matchDtos = _mapper.Map<List<MatchDto>>(matches);
            return Result<List<MatchDto>>.Success(matchDtos);
        }
    }
}

