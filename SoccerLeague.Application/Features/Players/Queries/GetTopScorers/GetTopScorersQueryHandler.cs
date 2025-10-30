using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetTopScorers
{
    public class GetTopScorersQueryHandler : IRequestHandler<GetTopScorersQuery, Result<List<PlayerDto>>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public GetTopScorersQueryHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<PlayerDto>>> Handle(GetTopScorersQuery request, CancellationToken cancellationToken)
        {
            var players = await _repository.GetTopScorersAsync(request.Count);
            var playerDtos = _mapper.Map<List<PlayerDto>>(players);

            return Result<List<PlayerDto>>.Success(playerDtos);
        }
    }
}
