using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetCurrentSeason
{
    public class GetCurrentSeasonQueryHandler : IRequestHandler<GetCurrentSeasonQuery, Result<SeasonDto>>
    {
        private readonly ISeasonRepository _repository;
        private readonly IMapper _mapper;

        public GetCurrentSeasonQueryHandler(ISeasonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonDto>> Handle(GetCurrentSeasonQuery request, CancellationToken cancellationToken)
        {
            var season = await _repository.GetCurrentSeasonAsync();
            if (season == null)
            {
                return Result<SeasonDto>.Failure("No current season found");
            }

            var seasonDto = _mapper.Map<SeasonDto>(season);
            return Result<SeasonDto>.Success(seasonDto);
        }
    }
}
