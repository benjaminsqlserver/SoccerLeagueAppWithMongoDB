using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.UserSession;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.UserSessions.Commands.CreateUserSession
{
    public class CreateUserSessionCommandHandler : IRequestHandler<CreateUserSessionCommand, Result<UserSessionDto>>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IMapper _mapper;

        public CreateUserSessionCommandHandler(IUserSessionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<UserSessionDto>> Handle(CreateUserSessionCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateUserSessionCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<UserSessionDto>.Failure(errors);
            }

            var session = _mapper.Map<UserSession>(request.Session);
            session.CreatedDate = DateTime.UtcNow;

            var createdSession = await _repository.AddAsync(session);
            var sessionDto = _mapper.Map<UserSessionDto>(createdSession);

            return Result<UserSessionDto>.Success(sessionDto);
        }
    }
}
