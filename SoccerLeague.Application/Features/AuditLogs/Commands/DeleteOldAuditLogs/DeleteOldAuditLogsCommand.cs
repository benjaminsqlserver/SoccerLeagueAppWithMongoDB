using MediatR;
using SoccerLeague.Application.Common.Models;

namespace SoccerLeague.Application.Features.AuditLogs.Commands.DeleteOldAuditLogs
{
    public class DeleteOldAuditLogsCommand : IRequest<Result<bool>>
    {
        public DateTime OlderThan { get; set; }
    }
}
