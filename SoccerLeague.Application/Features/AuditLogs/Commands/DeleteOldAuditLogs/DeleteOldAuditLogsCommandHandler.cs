using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.AuditLogs.Commands.DeleteOldAuditLogs
{
    public class DeleteOldAuditLogsCommandHandler : IRequestHandler<DeleteOldAuditLogsCommand, Result<bool>>
    {
        private readonly IAuditLogRepository _repository;

        public DeleteOldAuditLogsCommandHandler(IAuditLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteOldAuditLogsCommand request, CancellationToken cancellationToken)
        {
            if (request.OlderThan >= DateTime.UtcNow)
            {
                return Result<bool>.Failure("Date must be in the past");
            }

            var result = await _repository.DeleteOldAuditLogsAsync(request.OlderThan);

            if (!result)
            {
                return Result<bool>.Failure("Failed to delete old audit logs");
            }

            return Result<bool>.Success(true);
        }
    }
}
