using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Commands.UpdateTeam
{
    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Result<TeamDto>>
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public UpdateTeamCommandHandler(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamDto>> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeamCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<TeamDto>.Failure(errors);
            }

            var existingTeam = await _repository.GetByIdAsync(request.Team.Id);
            if (existingTeam == null)
            {
                return Result<TeamDto>.Failure("Team not found");
            }

            _mapper.Map(request.Team, existingTeam);
            existingTeam.ModifiedDate = DateTime.UtcNow;

            var updatedTeam = await _repository.UpdateAsync(existingTeam);
            var teamDto = _mapper.Map<TeamDto>(updatedTeam);

            return Result<TeamDto>.Success(teamDto);
        }
    }
}