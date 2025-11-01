using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByEntity
{
    public class GetAuditLogsByEntityQuery : IRequest<Result<List<AuditLogDto>>>
    {
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }
}
