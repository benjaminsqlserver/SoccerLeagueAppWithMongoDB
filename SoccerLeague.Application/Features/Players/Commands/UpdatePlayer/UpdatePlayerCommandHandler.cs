using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Player;

namespace SoccerLeague.Application.Features.Players.Commands.UpdatePlayer
{
    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Result<PlayerDto>>
    {
        private readonly IPlayerRepository _repository;
        private readonly IMapper _mapper;

        public UpdatePlayerCommandHandler(IPlayerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerDto>> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdatePlayerCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<PlayerDto>.Failure(errors);
            }

            var existingPlayer = await _repository.GetByIdAsync(request.Player.Id);
            if (existingPlayer == null)
            {
                return Result<PlayerDto>.Failure("Player not found");
            }

            _mapper.Map(request.Player, existingPlayer);
            existingPlayer.ModifiedDate = DateTime.UtcNow;

            var updatedPlayer = await _repository.UpdateAsync(existingPlayer);
            var playerDto = _mapper.Map<PlayerDto>(updatedPlayer);

            return Result<PlayerDto>.Success(playerDto);
        }
    }
}
