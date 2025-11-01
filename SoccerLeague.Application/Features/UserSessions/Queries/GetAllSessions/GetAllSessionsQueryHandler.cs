using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.UserSession;

namespace SoccerLeague.Application.Features.UserSessions.Queries.GetAllSessions
{
    public class GetAllSessionsQueryHandler : IRequestHandler<GetAllSessionsQuery, Result<PagedResult<UserSessionDto>>>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSessionsQueryHandler(IUserSessionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<UserSessionDto>>> Handle(GetAllSessionsQuery request, CancellationToken cancellationToken)
        {
            var pagedSessions = await _repository.GetSessionsAsync(request.Parameters);
            var sessionDtos = _mapper.Map<List<UserSessionDto>>(pagedSessions.Items);

            var pagedResult = new PagedResult<UserSessionDto>
            {
                Items = sessionDtos,
                PageNumber = pagedSessions.PageNumber,
                PageSize = pagedSessions.PageSize,
                TotalCount = pagedSessions.TotalCount
            };

            return Result<PagedResult<UserSessionDto>>.Success(pagedResult);
        }
    }
}
