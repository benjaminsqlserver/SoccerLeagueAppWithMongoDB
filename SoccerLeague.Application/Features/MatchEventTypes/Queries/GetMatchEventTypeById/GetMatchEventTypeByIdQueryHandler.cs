using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetMatchEventTypeById
{
    public class GetMatchEventTypeByIdQueryHandler : IRequestHandler<GetMatchEventTypeByIdQuery, Result<MatchEventTypeDto>>
    {
        private readonly IMatchEventTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetMatchEventTypeByIdQueryHandler(IMatchEventTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchEventTypeDto>> Handle(GetMatchEventTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var eventType = await _repository.GetByIdAsync(request.Id);
            if (eventType == null)
            {
                return Result<MatchEventTypeDto>.Failure("Match event type not found");
            }

            var eventTypeDto = _mapper.Map<MatchEventTypeDto>(eventType);
            return Result<MatchEventTypeDto>.Success(eventTypeDto);
        }
    }
}
