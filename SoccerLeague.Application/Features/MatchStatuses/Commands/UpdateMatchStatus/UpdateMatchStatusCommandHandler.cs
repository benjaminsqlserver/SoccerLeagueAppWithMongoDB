using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Commands.UpdateMatchStatus
{
    public class UpdateMatchStatusCommandHandler : IRequestHandler<UpdateMatchStatusCommand, Result<MatchStatusDto>>
    {
        private readonly IMatchStatusRepository _repository;
        private readonly IMapper _mapper;

        public UpdateMatchStatusCommandHandler(IMatchStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchStatusDto>> Handle(UpdateMatchStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateMatchStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchStatusDto>.Failure(errors);
            }

            var existingStatus = await _repository.GetByIdAsync(request.MatchStatus.Id);
            if (existingStatus == null)
            {
                return Result<MatchStatusDto>.Failure("Match status not found");
            }

            _mapper.Map(request.MatchStatus, existingStatus);
            existingStatus.ModifiedDate = DateTime.UtcNow;

            var updatedStatus = await _repository.UpdateAsync(existingStatus);
            var statusDto = _mapper.Map<MatchStatusDto>(updatedStatus);

            return Result<MatchStatusDto>.Success(statusDto);
        }
    }
}
