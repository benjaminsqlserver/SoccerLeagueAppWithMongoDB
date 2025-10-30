using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.TeamStatus;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.TeamStatuses.Commands.CreateTeamStatus
{
    public class CreateTeamStatusCommandHandler : IRequestHandler<CreateTeamStatusCommand, Result<TeamStatusDto>>
    {
        private readonly ITeamStatusRepository _repository;
        private readonly IMapper _mapper;

        public CreateTeamStatusCommandHandler(ITeamStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamStatusDto>> Handle(CreateTeamStatusCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateTeamStatusCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<TeamStatusDto>.Failure(errors);
            }

            var teamStatus = _mapper.Map<TeamStatus>(request.TeamStatus);
            teamStatus.CreatedDate = DateTime.UtcNow;

            var createdStatus = await _repository.AddAsync(teamStatus);
            var statusDto = _mapper.Map<TeamStatusDto>(createdStatus);

            return Result<TeamStatusDto>.Success(statusDto);
        }
    }
}
