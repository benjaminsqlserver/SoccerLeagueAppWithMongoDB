using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogsByUser
{
    public class GetAuditLogsByUserQuery : IRequest<Result<List<AuditLogDto>>>
    {
        public string UserId { get; set; } = string.Empty;
        public int Limit { get; set; } = 100;
    }
}
