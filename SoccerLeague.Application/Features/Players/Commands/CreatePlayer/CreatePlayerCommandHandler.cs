using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Players.Commands.CreatePlayer
{
    public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, Result<PlayerDto>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public CreatePlayerCommandHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerDto>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatePlayerCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<PlayerDto>.Failure(errors);
            }

            var player = _mapper.Map<Player>(request.Player);
            player.CreatedDate = DateTime.UtcNow;

            var createdPlayer = await _repository.AddAsync(player);
            var playerDto = _mapper.Map<PlayerDto>(createdPlayer);

            return Result<PlayerDto>.Success(playerDto);
        }
    }
}
