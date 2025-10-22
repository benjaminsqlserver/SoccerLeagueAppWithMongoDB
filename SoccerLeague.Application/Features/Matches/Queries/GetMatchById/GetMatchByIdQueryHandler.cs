using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetMatchById
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class GetMatchByIdQueryHandler : IRequestHandler<GetMatchByIdQuery, Result<MatchDto>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetMatchByIdQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<MatchDto>> Handle(GetMatchByIdQuery request, CancellationToken cancellationToken)
        {
            var match = await _matchRepository.GetMatchWithDetailsAsync(request.MatchId);

            if (match == null)
            {
                return Result<MatchDto>.Failure("Match not found");
            }

            var matchDto = _mapper.Map<MatchDto>(match);
            return Result<MatchDto>.Success(matchDto);
        }
    }
}

