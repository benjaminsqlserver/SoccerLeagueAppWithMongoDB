using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Queries.GetAllPlayers
{
    public class GetAllPlayersQueryHandler : IRequestHandler<GetAllPlayersQuery, Result<PagedResult<PlayerDto>>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPlayersQueryHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<PlayerDto>>> Handle(GetAllPlayersQuery request, CancellationToken cancellationToken)
        {
            var pagedPlayers = await _repository.GetPlayersAsync(request.Parameters);
            var playerDtos = _mapper.Map<List<PlayerDto>>(pagedPlayers.Items);

            var pagedResult = new PagedResult<PlayerDto>
            {
                Items = playerDtos,
                PageNumber = pagedPlayers.PageNumber,
                PageSize = pagedPlayers.PageSize,
                TotalCount = pagedPlayers.TotalCount
            };

            return Result<PagedResult<PlayerDto>>.Success(pagedResult);
        }
    }
}
