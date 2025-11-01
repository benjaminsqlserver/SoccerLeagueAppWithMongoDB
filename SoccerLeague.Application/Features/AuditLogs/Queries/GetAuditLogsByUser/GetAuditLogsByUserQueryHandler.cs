using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByUser
{
    public class GetAuditLogsByUserQueryHandler : IRequestHandler<GetAuditLogsByUserQuery, Result<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetAuditLogsByUserQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogsByUserQuery request, CancellationToken cancellationToken)
        {
            var auditLogs = await _repository.GetAuditLogsByUserAsync(request.UserId, request.Limit);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(auditLogs);

            return Result<List<AuditLogDto>>.Success(auditLogDtos);
        }
    }
}
