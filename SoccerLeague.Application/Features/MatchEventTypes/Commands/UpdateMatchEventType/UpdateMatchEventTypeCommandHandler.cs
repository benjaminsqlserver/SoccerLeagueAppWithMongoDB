using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchEventType;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.UpdateMatchEventType
{
    public class UpdateMatchEventTypeCommandHandler : IRequestHandler<UpdateMatchEventTypeCommand, Result<MatchEventTypeDto>>
    {
        private readonly IMatchEventTypeRepository _repository;
        private readonly IMapper _mapper;

        public UpdateMatchEventTypeCommandHandler(IMatchEventTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchEventTypeDto>> Handle(UpdateMatchEventTypeCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateMatchEventTypeCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchEventTypeDto>.Failure(errors);
            }

            var existingEventType = await _repository.GetByIdAsync(request.MatchEventType.Id);
            if (existingEventType == null)
            {
                return Result<MatchEventTypeDto>.Failure("Match event type not found");
            }

            _mapper.Map(request.MatchEventType, existingEventType);
            existingEventType.ModifiedDate = DateTime.UtcNow;

            var updatedEventType = await _repository.UpdateAsync(existingEventType);
            var eventTypeDto = _mapper.Map<MatchEventTypeDto>(updatedEventType);

            return Result<MatchEventTypeDto>.Success(eventTypeDto);
        }
    }
}
