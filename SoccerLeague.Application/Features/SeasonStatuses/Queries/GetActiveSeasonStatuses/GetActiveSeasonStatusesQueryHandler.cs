using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetActiveSeasonStatuses
{
    public class GetActiveSeasonStatusesQueryHandler : IRequestHandler<GetActiveSeasonStatusesQuery, Result<List<SeasonStatusDto>>>
    {
        private readonly ISeasonStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveSeasonStatusesQueryHandler(ISeasonStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<SeasonStatusDto>>> Handle(GetActiveSeasonStatusesQuery request, CancellationToken cancellationToken)
        {
            var statuses = await _repository.GetActiveStatusesAsync();
            var statusDtos = _mapper.Map<List<SeasonStatusDto>>(statuses);

            return Result<List<SeasonStatusDto>>.Success(statusDtos);
        }
    }
}
