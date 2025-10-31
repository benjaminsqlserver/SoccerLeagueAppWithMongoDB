using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository _repository;

        public ChangePasswordCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var validator = new ChangePasswordCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<bool>.Failure(errors);
            }

            var user = await _repository.GetByIdAsync(request.PasswordData.UserId);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.PasswordData.CurrentPassword, user.PasswordHash))
            {
                return Result<bool>.Failure("Current password is incorrect");
            }

            // Hash new password
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordData.NewPassword);

            var result = await _repository.UpdatePasswordAsync(request.PasswordData.UserId, newPasswordHash);
            if (!result)
            {
                return Result<bool>.Failure("Failed to update password");
            }

            return Result<bool>.Success(true);
        }
    }
}
