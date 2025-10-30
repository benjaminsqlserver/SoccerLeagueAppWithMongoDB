using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetPlayersByTeam
{
    public class GetPlayersByTeamQueryHandler : IRequestHandler<GetPlayersByTeamQuery, Result<List<PlayerDto>>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public GetPlayersByTeamQueryHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<PlayerDto>>> Handle(GetPlayersByTeamQuery request, CancellationToken cancellationToken)
        {
            var players = await _repository.GetPlayersByTeamAsync(request.TeamId);
            var playerDtos = _mapper.Map<List<PlayerDto>>(players);

            return Result<List<PlayerDto>>.Success(playerDtos);
        }
    }
}
