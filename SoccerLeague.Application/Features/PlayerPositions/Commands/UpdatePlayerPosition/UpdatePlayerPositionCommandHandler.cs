using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.PlayerPosition;

namespace SoccerLeague.Application.Features.PlayerPositions.Commands.UpdatePlayerPosition
{
    public class UpdatePlayerPositionCommandHandler : IRequestHandler<UpdatePlayerPositionCommand, Result<PlayerPositionDto>>
    {
        private readonly IPlayerPositionRepository _repository;
        private readonly IMapper _mapper;

        public UpdatePlayerPositionCommandHandler(IPlayerPositionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PlayerPositionDto>> Handle(UpdatePlayerPositionCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdatePlayerPositionCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<PlayerPositionDto>.Failure(errors);
            }

            var existingPosition = await _repository.GetByIdAsync(request.PlayerPosition.Id);
            if (existingPosition == null)
            {
                return Result<PlayerPositionDto>.Failure("Player position not found");
            }

            _mapper.Map(request.PlayerPosition, existingPosition);
            existingPosition.ModifiedDate = DateTime.UtcNow;

            var updatedPosition = await _repository.UpdateAsync(existingPosition);
            var positionDto = _mapper.Map<PlayerPositionDto>(updatedPosition);

            return Result<PlayerPositionDto>.Success(positionDto);
        }
    }
}
