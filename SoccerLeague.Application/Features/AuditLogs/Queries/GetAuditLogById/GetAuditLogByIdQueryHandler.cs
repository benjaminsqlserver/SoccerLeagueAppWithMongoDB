using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;

namespace SoccerLeague.Application.Features.AuditLogs.Queries.GetAuditLogById
{
    public class GetAuditLogByIdQueryHandler : IRequestHandler<GetAuditLogByIdQuery, Result<AuditLogDto>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public GetAuditLogByIdQueryHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<AuditLogDto>> Handle(GetAuditLogByIdQuery request, CancellationToken cancellationToken)
        {
            var auditLog = await _repository.GetByIdAsync(request.Id);

            if (auditLog == null)
            {
                return Result<AuditLogDto>.Failure("Audit log not found");
            }

            var auditLogDto = _mapper.Map<AuditLogDto>(auditLog);
            return Result<AuditLogDto>.Success(auditLogDto);
        }
    }
}

