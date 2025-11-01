using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Commands.CreateAuditLog
{
    public class CreateAuditLogCommand : IRequest<Result<AuditLogDto>>
    {
        public CreateAuditLogDto AuditLog { get; set; } = null!;
    }
}
