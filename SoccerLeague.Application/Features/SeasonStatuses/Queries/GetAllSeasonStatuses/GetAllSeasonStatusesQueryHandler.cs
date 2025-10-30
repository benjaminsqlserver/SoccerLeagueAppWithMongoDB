using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetAllSeasonStatuses
{
    public class GetAllSeasonStatusesQueryHandler : IRequestHandler<GetAllSeasonStatusesQuery, Result<PagedResult<SeasonStatusDto>>>
    {
        private readonly ISeasonStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSeasonStatusesQueryHandler(ISeasonStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<SeasonStatusDto>>> Handle(GetAllSeasonStatusesQuery request, CancellationToken cancellationToken)
        {
            var pagedStatuses = await _repository.GetPagedAsync(request.Parameters);
            var statusDtos = _mapper.Map<List<SeasonStatusDto>>(pagedStatuses.Items);

            var pagedResult = new PagedResult<SeasonStatusDto>
            {
                Items = statusDtos,
                PageNumber = pagedStatuses.PageNumber,
                PageSize = pagedStatuses.PageSize,
                TotalCount = pagedStatuses.TotalCount
            };

            return Result<PagedResult<SeasonStatusDto>>.Success(pagedResult);
        }
    }
}
