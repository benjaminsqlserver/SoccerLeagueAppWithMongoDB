using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoccerLeague.Application.Features.Matches.Commands.AddMatchEvent
{
    using AutoMapper;
    using MediatR;
    using SoccerLeague.Application.Common.Models;
    using SoccerLeague.Application.Contracts.Persistence;
    using SoccerLeague.Domain.Entities;

    public class AddMatchEventCommandHandler : IRequestHandler<AddMatchEventCommand, Result<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMapper _mapper;

        public AddMatchEventCommandHandler(IMatchRepository matchRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _mapper = mapper;
        }

        public async Task<Result<bool>> Handle(AddMatchEventCommand request, CancellationToken cancellationToken)
        {
            var validator = new AddMatchEventCommandValidator(_matchRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            var matchEvent = _mapper.Map<MatchEvent>(request.MatchEvent);
            var result = await _matchRepository.AddMatchEventAsync(request.MatchEvent.MatchId, matchEvent);

            if (!result)
            {
                return Result<bool>.Failure("Failed to add match event");
            }

            return Result<bool>.Success(true);
        }
    }
}

