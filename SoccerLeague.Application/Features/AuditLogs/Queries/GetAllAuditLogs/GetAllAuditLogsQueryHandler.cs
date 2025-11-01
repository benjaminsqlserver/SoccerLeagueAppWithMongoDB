using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQueryHandler : IRequestHandler<GetAllAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetAllAuditLogsQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAllAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var pagedAuditLogs = await _repository.GetAuditLogsAsync(request.Parameters);
            var auditLogDtos = _mapper.Map<List<AuditLogDto>>(pagedAuditLogs.Items);

            var pagedResult = new PagedResult<AuditLogDto>
            {
                Items = auditLogDtos,
                PageNumber = pagedAuditLogs.PageNumber,
                PageSize = pagedAuditLogs.PageSize,
                TotalCount = pagedAuditLogs.TotalCount
            };

            return Result<PagedResult<AuditLogDto>>.Success(pagedResult);
        }
    }
}
