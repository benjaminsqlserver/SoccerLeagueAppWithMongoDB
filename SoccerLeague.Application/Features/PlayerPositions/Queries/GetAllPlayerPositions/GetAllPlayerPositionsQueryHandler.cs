using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetAllPlayerPositions
{
    public class GetAllPlayerPositionsQueryHandler : IRequestHandler<GetAllPlayerPositionsQuery, Result<PagedResult<PlayerPositionDto>>>
    {
        private readonly IPlayerPositionRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPlayerPositionsQueryHandler(IPlayerPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<PlayerPositionDto>>> Handle(GetAllPlayerPositionsQuery request, CancellationToken cancellationToken)
        {
            var pagedPositions = await _repository.GetPagedAsync(request.Parameters);
            var positionDtos = _mapper.Map<List<PlayerPositionDto>>(pagedPositions.Items);

            var pagedResult = new PagedResult<PlayerPositionDto>
            {
                Items = positionDtos,
                PageNumber = pagedPositions.PageNumber,
                PageSize = pagedPositions.PageSize,
                TotalCount = pagedPositions.TotalCount
            };

            return Result<PagedResult<PlayerPositionDto>>.Success(pagedResult);
        }
    }
}
