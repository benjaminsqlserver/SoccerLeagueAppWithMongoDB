using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;
using System.Collections.Generic;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetFailedActions
{
    public class GetFailedActionsQuery : IRequest<Result<List<AuditLogDto>>>
    {
        public int Limit { get; set; } = 100;
    }
}
