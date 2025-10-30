using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Season;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Seasons.Commands.CreateSeason
{
    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, Result<SeasonDto>>
    {
        private readonly ISeasonRepository _repository;
        private readonly IMapper _mapper;

        public CreateSeasonCommandHandler(ISeasonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonDto>> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateSeasonCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SeasonDto>.Failure(errors);
            }

            var season = _mapper.Map<Season>(request.Season);
            season.CreatedDate = DateTime.UtcNow;

            var createdSeason = await _repository.AddAsync(season);
            var seasonDto = _mapper.Map<SeasonDto>(createdSeason);

            return Result<SeasonDto>.Success(seasonDto);
        }
    }
}
