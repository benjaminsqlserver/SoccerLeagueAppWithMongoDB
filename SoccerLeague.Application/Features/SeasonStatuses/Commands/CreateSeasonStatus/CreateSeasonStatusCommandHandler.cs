using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.SeasonStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.SeasonStatuses.Commands.CreateSeasonStatus
{
    public class CreateSeasonStatusCommandHandler : IRequestHandler<CreateSeasonStatusCommand, Result<SeasonStatusDto>>
    {
        private readonly ISeasonStatusRepository _repository;
        private readonly IMapper _mapper;

        public CreateSeasonStatusCommandHandler(ISeasonStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonStatusDto>> Handle(CreateSeasonStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateSeasonStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SeasonStatusDto>.Failure(errors);
            }

            var seasonStatus = _mapper.Map<SeasonStatus>(request.SeasonStatus);
            seasonStatus.CreatedDate = DateTime.UtcNow;

            var createdStatus = await _repository.AddAsync(seasonStatus);
            var statusDto = _mapper.Map<SeasonStatusDto>(createdStatus);

            return Result<SeasonStatusDto>.Success(statusDto);
        }
    }
}
