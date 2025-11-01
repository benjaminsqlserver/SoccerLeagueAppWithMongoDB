using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetSessionById
{
    public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Result<UserSessionDto>>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IMapper _mapper;

        public GetSessionByIdQueryHandler(IUserSessionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<UserSessionDto>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
        {
            var session = await _repository.GetByIdAsync(request.Id);

            if (session == null)
            {
                return Result<UserSessionDto>.Failure("Session not found");
            }

            var sessionDto = _mapper.Map<UserSessionDto>(session);
            return Result<UserSessionDto>.Success(sessionDto);
        }
    }
}
