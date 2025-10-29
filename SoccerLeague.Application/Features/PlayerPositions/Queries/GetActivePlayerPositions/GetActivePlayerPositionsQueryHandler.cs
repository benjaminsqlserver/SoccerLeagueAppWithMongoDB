using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetActivePlayerPositions
{
    public class GetActivePlayerPositionsQueryHandler : IRequestHandler<GetActivePlayerPositionsQuery, Result<List<PlayerPositionDto>>>
    {
        private readonly IPlayerPositionRepository _repository;
        private readonly IMapper _mapper;

        public GetActivePlayerPositionsQueryHandler(IPlayerPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<PlayerPositionDto>>> Handle(GetActivePlayerPositionsQuery request, CancellationToken cancellationToken)
        {
            var positions = await _repository.GetActivePositionsAsync();
            var positionDtos = _mapper.Map<List<PlayerPositionDto>>(positions);

            return Result<List<PlayerPositionDto>>.Success(positionDtos);
        }
    }
}
