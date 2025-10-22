using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Queries.GetAllMatches
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class GetAllMatchesQueryHandler : IRequestHandler<GetAllMatchesQuery, Result<PagedResult<MatchDto>>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public GetAllMatchesQueryHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<MatchDto>>> Handle(GetAllMatchesQuery request, CancellationToken cancellationToken)
        {
            var pagedMatches = await _matchRepository.GetMatchesAsync(request.Parameters);

            var matchDtos = _mapper.Map<List<MatchDto>>(pagedMatches.Items);

            var pagedResult = new PagedResult<MatchDto>
            {
                Items = matchDtos,
                PageNumber = pagedMatches.PageNumber,
                PageSize = pagedMatches.PageSize,
                TotalCount = pagedMatches.TotalCount
            };

            return Result<PagedResult<MatchDto>>.Success(pagedResult);
        }
    }
}


