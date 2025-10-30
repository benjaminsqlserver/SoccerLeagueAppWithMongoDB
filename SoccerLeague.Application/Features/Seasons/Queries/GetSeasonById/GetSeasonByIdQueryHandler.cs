using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetSeasonById
{
    public class GetSeasonByIdQueryHandler : IRequestHandler<GetSeasonByIdQuery, Result<SeasonDto>>
    {
        private readonly ISeasonRepository _repository;
        private readonly IMapper _mapper;

        public GetSeasonByIdQueryHandler(ISeasonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonDto>> Handle(GetSeasonByIdQuery request, CancellationToken cancellationToken)
        {
            var season = await _repository.GetByIdAsync(request.Id);
            if (season == null)
            {
                return Result<SeasonDto>.Failure("Season not found");
            }

            var seasonDto = _mapper.Map<SeasonDto>(season);
            return Result<SeasonDto>.Success(seasonDto);
        }
    }
}
