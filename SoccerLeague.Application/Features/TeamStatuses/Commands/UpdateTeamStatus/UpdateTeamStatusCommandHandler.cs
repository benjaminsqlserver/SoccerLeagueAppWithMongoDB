using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.UpdateTeamStatus
{
    public class UpdateTeamStatusCommandHandler : IRequestHandler<UpdateTeamStatusCommand, Result<TeamStatusDto>>
    {
        private readonly ITeamStatusRepository _repository;
        private readonly IMapper _mapper;

        public UpdateTeamStatusCommandHandler(ITeamStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamStatusDto>> Handle(UpdateTeamStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeamStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<TeamStatusDto>.Failure(errors);
            }

            var existingStatus = await _repository.GetByIdAsync(request.TeamStatus.Id);
            if (existingStatus == null)
            {
                return Result<TeamStatusDto>.Failure("Team status not found");
            }

            _mapper.Map(request.TeamStatus, existingStatus);
            existingStatus.ModifiedDate = DateTime.UtcNow;

            var updatedStatus = await _repository.UpdateAsync(existingStatus);
            var statusDto = _mapper.Map<TeamStatusDto>(updatedStatus);

            return Result<TeamStatusDto>.Success(statusDto);
        }
    }
}
