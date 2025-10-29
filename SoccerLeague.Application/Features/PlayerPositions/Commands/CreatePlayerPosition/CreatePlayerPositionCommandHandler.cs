using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.PlayerPosition;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.CreatePlayerPosition
{
    public class CreatePlayerPositionCommandHandler : IRequestHandler<CreatePlayerPositionCommand, Result<PlayerPositionDto>>
    {
        private readonly IPlayerPositionRepository _repository;
        private readonly IMapper _mapper;

        public CreatePlayerPositionCommandHandler(IPlayerPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerPositionDto>> Handle(CreatePlayerPositionCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatePlayerPositionCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<PlayerPositionDto>.Failure(errors);
            }

            var playerPosition = _mapper.Map<PlayerPosition>(request.PlayerPosition);
            playerPosition.CreatedDate = DateTime.UtcNow;

            var createdPosition = await _repository.AddAsync(playerPosition);
            var positionDto = _mapper.Map<PlayerPositionDto>(createdPosition);

            return Result<PlayerPositionDto>.Success(positionDto);
        }
    }
}
