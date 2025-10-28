using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.MatchStatus;

namespace SoccerLeague.Application.Features.MatchStatuses.Queries.GetMatchStatusById
{
    public class GetMatchStatusByIdQueryHandler : IRequestHandler<GetMatchStatusByIdQuery, Result<MatchStatusDto>>
    {
        private readonly IMatchStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetMatchStatusByIdQueryHandler(IMatchStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<MatchStatusDto>> Handle(GetMatchStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<MatchStatusDto>.Failure("Match status not found");
            }

            var statusDto = _mapper.Map<MatchStatusDto>(status);
            return Result<MatchStatusDto>.Success(statusDto);
        }
    }
}
