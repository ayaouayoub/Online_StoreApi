using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Common.Models;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.User.Queries;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Exceptions;

namespace OnlineStore.Application.Handlers.User
{
    public sealed class GetUsersHandler
    {
        private readonly IUserRepository _userRepository;

        public GetUsersHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResultDto<UserDto>> ExecuteAsync(GetUsersQuery query)
        {
            if (query.Page < 1) throw new DomainException("Page number must be greater than 0.");

            if (query.PageSize < 1 || query.PageSize > 100) throw new DomainException("Page size must be between 1 and 100.");

            var result = await _userRepository.GetUsersAsync(query);

            return new PagedResultDto<UserDto>
            {
                Items = result.Items,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }
    }
}
