using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetActiveUserSessions
{
    public class GetActiveUserSessionsQueryHandler : IRequestHandler<GetActiveUserSessionsQuery, Result<List<UserSessionDto>>>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveUserSessionsQueryHandler(IUserSessionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<UserSessionDto>>> Handle(GetActiveUserSessionsQuery request, CancellationToken cancellationToken)
        {
            var sessions = await _repository.GetActiveSessionsByUserAsync(request.UserId);
            var sessionDtos = _mapper.Map<List<UserSessionDto>>(sessions);

            return Result<List<UserSessionDto>>.Success(sessionDtos);
        }
    }
}
