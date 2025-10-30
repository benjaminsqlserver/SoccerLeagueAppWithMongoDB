using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.TeamStatus;

namespace SoccerLeague.Application.Features.TeamStatuses.Queries.GetTeamStatusById
{
    public class GetTeamStatusByIdQueryHandler : IRequestHandler<GetTeamStatusByIdQuery, Result<TeamStatusDto>>
    {
        private readonly ITeamStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetTeamStatusByIdQueryHandler(ITeamStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<TeamStatusDto>> Handle(GetTeamStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<TeamStatusDto>.Failure("Team status not found");
            }

            var statusDto = _mapper.Map<TeamStatusDto>(status);
            return Result<TeamStatusDto>.Success(statusDto);
        }
    }
}
