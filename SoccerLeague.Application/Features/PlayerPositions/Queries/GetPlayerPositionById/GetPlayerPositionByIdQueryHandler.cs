using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Queries.GetPlayerPositionById
{
    public class GetPlayerPositionByIdQueryHandler : IRequestHandler<GetPlayerPositionByIdQuery, Result<PlayerPositionDto>>
    {
        private readonly IPlayerPositionRepository _repository;
        private readonly IMapper _mapper;

        public GetPlayerPositionByIdQueryHandler(IPlayerPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerPositionDto>> Handle(GetPlayerPositionByIdQuery request, CancellationToken cancellationToken)
        {
            var position = await _repository.GetByIdAsync(request.Id);
            if (position == null)
            {
                return Result<PlayerPositionDto>.Failure("Player position not found");
            }

            var positionDto = _mapper.Map<PlayerPositionDto>(position);
            return Result<PlayerPositionDto>.Success(positionDto);
        }
    }
}
