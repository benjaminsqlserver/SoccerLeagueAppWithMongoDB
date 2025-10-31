using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SoccerLeague.Application.Common.Models;
using SoccerLeague.Application.Contracts.Persistence;
using SoccerLeague.Application.DTOs.User;

namespace SoccerLeague.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedResult<UserDto>>>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<PagedResult<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var pagedUsers = await _repository.GetUsersAsync(request.Parameters);
            var userDtos = _mapper.Map<List<UserDto>>(pagedUsers.Items);

            var pagedResult = new PagedResult<UserDto>
            {
                Items = userDtos,
                PageNumber = pagedUsers.PageNumber,
                PageSize = pagedUsers.PageSize,
                TotalCount = pagedUsers.TotalCount
            };

            return Result<PagedResult<UserDto>>.Success(pagedResult);
        }
    }
}
