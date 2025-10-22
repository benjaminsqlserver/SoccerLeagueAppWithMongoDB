using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.UpdateMatch
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;

    public class UpdateMatchCommandHandler : IRequestHandler<UpdateMatchCommand, Result<MatchDto>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public UpdateMatchCommandHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<MatchDto>> Handle(UpdateMatchCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateMatchCommandValidator(_matchRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchDto>.Failure(errors);
            }

            var existingMatch = await _matchRepository.GetByIdAsync(request.Match.Id);
            if (existingMatch == null)
            {
                return Result<MatchDto>.Failure("Match not found");
            }

            _mapper.Map(request.Match, existingMatch);
            existingMatch.ModifiedDate = DateTime.UtcNow;

            var updatedMatch = await _matchRepository.UpdateAsync(existingMatch);
            var matchDto = _mapper.Map<MatchDto>(updatedMatch);

            return Result<MatchDto>.Success(matchDto);
        }
    }
}
