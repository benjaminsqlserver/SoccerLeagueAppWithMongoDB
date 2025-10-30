using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.SeasonStatus;

namespace SoccerLeague.Application.Features.SeasonStatuses.Queries.GetSeasonStatusById
{
    public class GetSeasonStatusByIdQueryHandler : IRequestHandler<GetSeasonStatusByIdQuery, Result<SeasonStatusDto>>
    {
        private readonly ISeasonStatusRepository _repository;
        private readonly IMapper _mapper;

        public GetSeasonStatusByIdQueryHandler(ISeasonStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonStatusDto>> Handle(GetSeasonStatusByIdQuery request, CancellationToken cancellationToken)
        {
            var status = await _repository.GetByIdAsync(request.Id);
            if (status == null)
            {
                return Result<SeasonStatusDto>.Failure("Season status not found");
            }

            var statusDto = _mapper.Map<SeasonStatusDto>(status);
            return Result<SeasonStatusDto>.Success(statusDto);
        }
    }
}
