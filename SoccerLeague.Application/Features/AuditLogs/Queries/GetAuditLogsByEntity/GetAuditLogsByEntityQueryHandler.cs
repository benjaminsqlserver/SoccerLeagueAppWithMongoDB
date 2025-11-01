using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity
{
    public class GetAuditLogsByEntityQueryHandler : IRequestHandler<GetAuditLogsByEntityQuery, Result<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetAuditLogsByEntityQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogsByEntityQuery request, CancellationToken cancellationToken)
        {
            var auditLogs = await _repository.GetAuditLogsByEntityAsync(request.EntityType, request.EntityId);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(auditLogs);

            return Result<List<AuditLogDto>>.Success(auditLogDtos);
        }
    }
}
