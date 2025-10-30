using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.UpdateSeasonStatus
{
    public class UpdateSeasonStatusCommandHandler : IRequestHandler<UpdateSeasonStatusCommand, Result<SeasonStatusDto>>
    {
        private readonly ISeasonStatusRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSeasonStatusCommandHandler(ISeasonStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonStatusDto>> Handle(UpdateSeasonStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateSeasonStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SeasonStatusDto>.Failure(errors);
            }

            var existingStatus = await _repository.GetByIdAsync(request.SeasonStatus.Id);
            if (existingStatus == null)
            {
                return Result<SeasonStatusDto>.Failure("Season status not found");
            }

            _mapper.Map(request.SeasonStatus, existingStatus);
            existingStatus.ModifiedDate = DateTime.UtcNow;

            var updatedStatus = await _repository.UpdateAsync(existingStatus);
            var statusDto = _mapper.Map<SeasonStatusDto>(updatedStatus);

            return Result<SeasonStatusDto>.Success(statusDto);
        }
    }
}
