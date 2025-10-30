using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Standing;

namespace SoccerLeague.Application.Features.Standings.Queries.GetStandingById
{
    public class GetStandingByIdQueryHandler : IRequestHandler<GetStandingByIdQuery, Result<StandingDto>>
    {
        private readonly IStandingRepository _repository;
        private readonly IMapper _mapper;

        public GetStandingByIdQueryHandler(IStandingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<StandingDto>> Handle(GetStandingByIdQuery request, CancellationToken cancellationToken)
        {
            var standing = await _repository.GetByIdAsync(request.Id);
            if (standing == null)
            {
                return Result<StandingDto>.Failure("Standing not found");
            }

            var standingDto = _mapper.Map<StandingDto>(standing);
            return Result<StandingDto>.Success(standingDto);
        }
    }
}
