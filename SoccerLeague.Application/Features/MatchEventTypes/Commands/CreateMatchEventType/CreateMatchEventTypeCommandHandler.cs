using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchEventType;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.MatchEventTypes.Commands.CreateMatchEventType
{
    public class CreateMatchEventTypeCommandHandler : IRequestHandler<CreateMatchEventTypeCommand, Result<MatchEventTypeDto>>
    {
        private readonly IMatchEventTypeRepository _repository;
        private readonly IMapper _mapper;

        public CreateMatchEventTypeCommandHandler(IMatchEventTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchEventTypeDto>> Handle(CreateMatchEventTypeCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateMatchEventTypeCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchEventTypeDto>.Failure(errors);
            }

            var eventType = _mapper.Map<MatchEventType>(request.MatchEventType);
            eventType.CreatedDate = DateTime.UtcNow;

            var createdEventType = await _repository.AddAsync(eventType);
            var eventTypeDto = _mapper.Map<MatchEventTypeDto>(createdEventType);

            return Result<MatchEventTypeDto>.Success(eventTypeDto);
        }
    }
}
