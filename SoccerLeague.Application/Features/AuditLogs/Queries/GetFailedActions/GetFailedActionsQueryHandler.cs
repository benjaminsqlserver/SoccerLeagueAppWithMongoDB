using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetFailedActions
{
    public class GetFailedActionsQueryHandler : IRequestHandler<GetFailedActionsQuery, Result<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetFailedActionsQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetFailedActionsQuery request, CancellationToken cancellationToken)
        {
            var auditLogs = await _repository.GetFailedActionsAsync(request.Limit);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(auditLogs);

            return Result<List<AuditLogDto>>.Success(auditLogDtos);
        }
    }
}
