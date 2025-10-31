using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;

namespace SoccerLeague.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly IUserRepository _repository;

        public DeleteUserCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Result<bool>.Failure("User not found");
            }

            var result = await _repository.DeleteAsync(request.Id);
            if (!result)
            {
                return Result<bool>.Failure("Failed to delete user");
            }

            return Result<bool>.Success(true);
        }
    }
}
