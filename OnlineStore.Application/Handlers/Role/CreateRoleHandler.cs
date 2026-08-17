using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Role.Commands;
using OnlineStore.Application.Handlers.Role.Mappings;
using OnlineStore.Application.Handlers.Role.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Role
{
    public sealed class CreateRoleHandler
    {
        private readonly IRoleRepository _roleRepository;

        public CreateRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> ExecuteAsync(CreateRoleCommand command)
        {
            var role = Domain.Entities.Role.Create(command.RoleName);
            var createdRole = await _roleRepository.CreateAsync(role);
            return createdRole.ToDto();
        }
    }
}
