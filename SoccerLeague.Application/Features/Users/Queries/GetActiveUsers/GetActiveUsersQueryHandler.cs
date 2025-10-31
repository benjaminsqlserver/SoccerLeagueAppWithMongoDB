using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Queries.GetActiveUsers
{
    public class GetActiveUsersQueryHandler : IRequestHandler<GetActiveUsersQuery, Result<List<UserDto>>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveUsersQueryHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<List<UserDto>>> Handle(GetActiveUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _repository.GetActiveUsersAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);
            return Result<List<UserDto>>.Success(userDtos);
        }
    }
}
