using FluentValidation;

namespace SoccerLeague.Application.Features.AuditLogs.Commands.CreateAuditLog
{
    public class CreateAuditLogCommandValidator : AbstractValidator<CreateAuditLogCommand>
    {
        public CreateAuditLogCommandValidator()
        {
            RuleFor(x => x.AuditLog.ActionType)
                .IsInEnum().WithMessage("Invalid action type");

            RuleFor(x => x.AuditLog.EntityType)
                .NotEmpty().WithMessage("Entity type is required")
                .MaximumLength(100).WithMessage("Entity type cannot exceed 100 characters");

            RuleFor(x => x.AuditLog.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.AuditLog.Username)
                .MaximumLength(256).When(x => !string.IsNullOrEmpty(x.AuditLog.Username))
                .WithMessage("Username cannot exceed 256 characters");

            RuleFor(x => x.AuditLog.IpAddress)
                .MaximumLength(45).When(x => !string.IsNullOrEmpty(x.AuditLog.IpAddress))
                .WithMessage("IP address cannot exceed 45 characters");
        }
    }
}
