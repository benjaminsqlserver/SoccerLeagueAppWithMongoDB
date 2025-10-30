using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Commands.UpdateStanding
{
    public class UpdateStandingCommandHandler : IRequestHandler<UpdateStandingCommand, Result<StandingDto>>
    {
        private readonly IStandingRepository _repository;
        private readonly IMapper _mapper;

        public UpdateStandingCommandHandler(IStandingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<StandingDto>> Handle(UpdateStandingCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateStandingCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<StandingDto>.Failure(errors);
            }

            var existingStanding = await _repository.GetByIdAsync(request.Standing.Id);
            if (existingStanding == null)
            {
                return Result<StandingDto>.Failure("Standing not found");
            }

            _mapper.Map(request.Standing, existingStanding);
            existingStanding.ModifiedDate = DateTime.UtcNow;

            var updatedStanding = await _repository.UpdateAsync(existingStanding);
            var standingDto = _mapper.Map<StandingDto>(updatedStanding);

            return Result<StandingDto>.Success(standingDto);
        }
    }
}
