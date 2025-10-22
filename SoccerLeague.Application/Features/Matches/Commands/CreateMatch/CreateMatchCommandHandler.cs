using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.CreateMatch
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Application.DTOs.Match;
    using SoccerLeague.Domain.Entities;

    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, Result<MatchDto>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public CreateMatchCommandHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<MatchDto>> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateMatchCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<MatchDto>.Failure(errors);
            }

            var match = _mapper.Map<Match>(request.Match);
            match.CreatedDate = DateTime.UtcNow;

            var createdMatch = await _matchRepository.AddAsync(match);
            var matchDto = _mapper.Map<MatchDto>(createdMatch);

            return Result<MatchDto>.Success(matchDto);
        }
    }
}
