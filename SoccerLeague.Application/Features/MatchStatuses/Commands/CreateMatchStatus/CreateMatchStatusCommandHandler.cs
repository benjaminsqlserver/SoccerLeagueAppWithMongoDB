using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.CreateMatchStatus
{
    public class CreateMatchStatusCommandHandler : IRequestHandler<CreateMatchStatusCommand, Result<MatchStatusDto>>
    {
        private readonly IMatchStatusRepository _repository;
        private readonly IMapper _mapper;

        public CreateMatchStatusCommandHandler(IMatchStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchStatusDto>> Handle(CreateMatchStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateMatchStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchStatusDto>.Failure(errors);
            }

            var matchStatus = _mapper.Map<MatchStatus>(request.MatchStatus);
            matchStatus.CreatedDate = DateTime.UtcNow;

            var createdStatus = await _repository.AddAsync(matchStatus);
            var statusDto = _mapper.Map<MatchStatusDto>(createdStatus);

            return Result<MatchStatusDto>.Success(statusDto);
        }
    }
}
