using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Queries.GetAllSeasons
{
    public class GetAllSeasonsQueryHandler : IRequestHandler<GetAllSeasonsQuery, Result<PagedResult<SeasonDto>>>
    {
        private readonly ISeasonRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSeasonsQueryHandler(ISeasonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<SeasonDto>>> Handle(GetAllSeasonsQuery request, CancellationToken cancellationToken)
        {
            var pagedSeasons = await _repository.GetSeasonsAsync(request.Parameters);
            var seasonDtos = _mapper.Map<List<SeasonDto>>(pagedSeasons.Items);

            var pagedResult = new PagedResult<SeasonDto>
            {
                Items = seasonDtos,
                PageNumber = pagedSeasons.PageNumber,
                PageSize = pagedSeasons.PageSize,
                TotalCount = pagedSeasons.TotalCount
            };

            return Result<PagedResult<SeasonDto>>.Success(pagedResult);
        }
    }
}
