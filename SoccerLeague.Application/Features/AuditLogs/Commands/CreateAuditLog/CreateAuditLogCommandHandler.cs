using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.AuditLog;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.AuditLogs.Commands.CreateAuditLog
{
    public class CreateAuditLogCommandHandler : IRequestHandler<CreateAuditLogCommand, Result<AuditLogDto>>
    {
        private readonly IAuditLogRepository _repository;
        private readonly IMapper _mapper;

        public CreateAuditLogCommandHandler(IAuditLogRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<AuditLogDto>> Handle(CreateAuditLogCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateAuditLogCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<AuditLogDto>.Failure(errors);
            }

            var auditLog = _mapper.Map<AuditLog>(request.AuditLog);
            auditLog.CreatedDate = DateTime.UtcNow;

            var createdAuditLog = await _repository.AddAsync(auditLog);
            var auditLogDto = _mapper.Map<AuditLogDto>(createdAuditLog);

            return Result<AuditLogDto>.Success(auditLogDto);
        }
    }
}
