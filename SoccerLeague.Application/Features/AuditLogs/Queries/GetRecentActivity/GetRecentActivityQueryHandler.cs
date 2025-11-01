using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetRecentActivity
{
    public class GetRecentActivityQueryHandler : IRequestHandler<GetRecentActivityQuery, Result<List<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetRecentActivityQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
        {
            var auditLogs = await _repository.GetRecentActivityAsync(request.Limit);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(auditLogs);

            return Result<List<AuditLogDto>>.Success(auditLogDtos);
        }
    }
}
