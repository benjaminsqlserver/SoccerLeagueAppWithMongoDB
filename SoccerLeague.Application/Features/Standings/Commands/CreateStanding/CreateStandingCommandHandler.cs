using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.Standing;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Standings.Commands.CreateStanding
{
    public class CreateStandingCommandHandler : IRequestHandler<CreateStandingCommand, Result<StandingDto>>
    {
        private readonly IStandingRepository _repository;
        private readonly IMapper _mapper;

        public CreateStandingCommandHandler(IStandingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<StandingDto>> Handle(CreateStandingCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateStandingCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<StandingDto>.Failure(errors);
            }

            var standing = _mapper.Map<Standing>(request.Standing);
            standing.CreatedDate = DateTime.UtcNow;

            var createdStanding = await _repository.AddAsync(standing);
            var standingDto = _mapper.Map<StandingDto>(createdStanding);

            return Result<StandingDto>.Success(standingDto);
        }
    }
}
