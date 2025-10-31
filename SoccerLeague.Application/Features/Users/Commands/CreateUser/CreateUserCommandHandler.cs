using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.User;
using SoccerLeague.Domain.Entities;

namespace SoccerLeague.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateUserCommandValidator(_repository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<UserDto>.Failure(errors);
            }

            var user = _mapper.Map<User>(request.User);

            // Hash password (simple implementation - in production use proper password hashing)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.User.Password);
            user.CreatedDate = DateTime.UtcNow;

            var createdUser = await _repository.AddAsync(user);
            var userDto = _mapper.Map<UserDto>(createdUser);

            return Result<UserDto>.Success(userDto);
        }
    }
}
