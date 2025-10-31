using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateUserCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<UserDto>.Failure(errors);
            }

            var existingUser = await _repository.GetByIdAsync(request.User.Id);
            if (existingUser == null)
            {
                return Result<UserDto>.Failure("User not found");
            }

            _mapper.Map(request.User, existingUser);
            existingUser.ModifiedDate = DateTime.UtcNow;

            var updatedUser = await _repository.UpdateAsync(existingUser);
            var userDto = _mapper.Map<UserDto>(updatedUser);

            return Result<UserDto>.Success(userDto);
        }
    }
}
