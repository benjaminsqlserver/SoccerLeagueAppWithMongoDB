using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Queries.GetAllMatchEventTypes
{
    public class GetAllMatchEventTypesQueryHandler : IRequestHandler<GetAllMatchEventTypesQuery, Result<PagedResult<MatchEventTypeDto>>>
    {
        private readonly IMatchEventTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMatchEventTypesQueryHandler(IMatchEventTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<MatchEventTypeDto>>> Handle(GetAllMatchEventTypesQuery request, CancellationToken cancellationToken)
        {
            var pagedEventTypes = await _repository.GetPagedAsync(request.Parameters);
            var eventTypeDtos = _mapper.Map<List<MatchEventTypeDto>>(pagedEventTypes.Items);

            var pagedResult = new PagedResult<MatchEventTypeDto>
            {
                Items = eventTypeDtos,
                PageNumber = pagedEventTypes.PageNumber,
                PageSize = pagedEventTypes.PageSize,
                TotalCount = pagedEventTypes.TotalCount
            };

            return Result<PagedResult<MatchEventTypeDto>>.Success(pagedResult);
        }
    }
}
