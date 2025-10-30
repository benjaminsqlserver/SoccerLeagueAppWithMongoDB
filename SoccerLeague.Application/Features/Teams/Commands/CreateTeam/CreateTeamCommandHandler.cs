using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Team;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Teams.Commands.CreateTeam
{
    public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Result<TeamDto>>
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public CreateTeamCommandHandler(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamDto>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateTeamCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<TeamDto>.Failure(errors);
            }

            var team = _mapper.Map<Team>(request.Team);
            team.CreatedDate = DateTime.UtcNow;

            var createdTeam = await _repository.AddAsync(team);
            var teamDto = _mapper.Map<TeamDto>(createdTeam);

            return Result<TeamDto>.Success(teamDto);
        }
    }
}
