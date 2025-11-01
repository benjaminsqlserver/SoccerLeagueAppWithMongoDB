using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAllAuditLogs
{
    public class GetAllAuditLogsQuery : IRequest<Result<PagedResult<AuditLogDto>>>
    {
        public AuditLogQueryParameters Parameters { get; set; } = new AuditLogQueryParameters();
    }
}
