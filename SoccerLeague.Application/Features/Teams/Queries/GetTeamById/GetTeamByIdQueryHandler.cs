using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Team;

namespace SoccerLeague.Application.Features.Teams.Queries.GetTeamById
{
    public class GetTeamByIdQueryHandler : IRequestHandler<GetTeamByIdQuery, Result<TeamDto>>
    {
        private readonly ITeamRepository _repository;
        private readonly IMapper _mapper;

        public GetTeamByIdQueryHandler(ITeamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamDto>> Handle(GetTeamByIdQuery request, CancellationToken cancellationToken)
        {
            var team = await _repository.GetTeamWithDetailsAsync(request.Id);
            if (team == null)
            {
                return Result<TeamDto>.Failure("Team not found");
            }

            var teamDto = _mapper.Map<TeamDto>(team);
            return Result<TeamDto>.Success(teamDto);
        }
    }
}
