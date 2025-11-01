using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogById
{
    public class GetAuditLogByIdQuery : IRequest<Result<AuditLogDto>>
    {
        public string Id { get; set; } = string.Empty;
    }
}
