using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetPlayerById
{
    public class GetPlayerByIdQueryHandler : IRequestHandler<GetPlayerByIdQuery, Result<PlayerDto>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public GetPlayerByIdQueryHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerDto>> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var player = await _repository.GetPlayerWithDetailsAsync(request.Id);
            if (player == null)
            {
                return Result<PlayerDto>.Failure("Player not found");
            }

            var playerDto = _mapper.Map<PlayerDto>(player);
            return Result<PlayerDto>.Success(playerDto);
        }
    }
}
