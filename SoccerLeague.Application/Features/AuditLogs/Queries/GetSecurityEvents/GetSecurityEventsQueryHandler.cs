using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetSecurityEvents
{
    public class GetSecurityEventsQueryHandler : IRequestHandler<GetSecurityEventsQuery, Result<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetSecurityEventsQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetSecurityEventsQuery request, CancellationToken cancellationToken)
        {
            var auditLogs = await _repository.GetSecurityEventsAsync(request.Limit);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(auditLogs);

            return Result<List<AuditLogDto>>.Success(auditLogDtos);
        }
    }
}
