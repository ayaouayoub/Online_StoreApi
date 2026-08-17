using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Role.Mappings;
using OnlineStore.Application.Handlers.Role.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Role
{
    public sealed class GetRoleHandler
    {
        private readonly IRoleRepository _roleRepository;

        public GetRoleHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> ExecuteAsync(GetRoleQuery query)
        {
            var role = await _roleRepository.GetByIdAsync(query.RoleId) ?? throw new NotFoundException("Role not found");
            return role.ToDto();
        }
    }
}
