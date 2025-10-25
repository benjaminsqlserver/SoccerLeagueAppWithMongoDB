using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetActiveMatchEventTypes
{
    public class GetActiveMatchEventTypesQueryHandler : IRequestHandler<GetActiveMatchEventTypesQuery, Result<List<MatchEventTypeDto>>>
    {
        private readonly IMatchEventTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveMatchEventTypesQueryHandler(IMatchEventTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<MatchEventTypeDto>>> Handle(GetActiveMatchEventTypesQuery request, CancellationToken cancellationToken)
        {
            var eventTypes = await _repository.GetActiveEventTypesAsync();
            var eventTypeDtos = _mapper.Map<List<MatchEventTypeDto>>(eventTypes);

            return Result<List<MatchEventTypeDto>>.Success(eventTypeDtos);
        }
    }
}
