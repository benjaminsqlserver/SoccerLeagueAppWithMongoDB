using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Season;

namespace SoccerLeague.Application.Features.Seasons.Commands.UpdateSeason
{
    public class UpdateSeasonCommandHandler : IRequestHandler<UpdateSeasonCommand, Result<SeasonDto>>
    {
        private readonly ISeasonRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSeasonCommandHandler(ISeasonRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<SeasonDto>> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateSeasonCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<SeasonDto>.Failure(errors);
            }

            var existingSeason = await _repository.GetByIdAsync(request.Season.Id);
            if (existingSeason == null)
            {
                return Result<SeasonDto>.Failure("Season not found");
            }

            // If setting as current season, unset other current seasons
            if (request.Season.IsCurrentSeason && !existingSeason.IsCurrentSeason)
            {
                await _repository.SetCurrentSeasonAsync(request.Season.Id);
            }

            _mapper.Map(request.Season, existingSeason);
            existingSeason.ModifiedDate = DateTime.UtcNow;

            var updatedSeason = await _repository.UpdateAsync(existingSeason);
            var seasonDto = _mapper.Map<SeasonDto>(updatedSeason);

            return Result<SeasonDto>.Success(seasonDto);
        }
    }
}
