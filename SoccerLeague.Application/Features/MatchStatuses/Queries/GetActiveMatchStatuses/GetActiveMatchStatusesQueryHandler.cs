using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetActiveMatchStatuses
{
    public class GetActiveMatchStatusesQueryHandler : IRequestHandler<GetActiveMatchStatusesQuery, Result<List<MatchStatusDto>>>
    {
        private readonly IMatchStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveMatchStatusesQueryHandler(IMatchStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<MatchStatusDto>>> Handle(GetActiveMatchStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _repository.GetActiveStatusesAsync();
            var statusDtos = _mapper.Map<List<MatchStatusDto>>(statuses);

            return Result<List<MatchStatusDto>>.Success(statusDtos);
        }
    }
}