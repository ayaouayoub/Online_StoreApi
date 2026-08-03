using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.User.Mappings;
using OnlineStore.Application.Handlers.User.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class GetUserHandler
    {
        private readonly IUserRepository _repo;

        public GetUserHandler(IUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserDto> ExecuteAsync(GetUserByIdQuery query)
        {
            var user = await _repo.GetByIdAsync(query.UserId) ?? throw new NotFoundException("User not found."); 
            return user.ToDto();
        }
    }
}
