
using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Role.Mappings;
using OnlineStore.Application.Handlers.Role.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Role
{
    public sealed class GetRolesHandler
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleDto>> ExecuteAsync(GetRolesQuery query)
        {
            var role = await _roleRepository.GetAllAsync();
            return [.. role.Select(r => r.ToDto())];
        }
    }
}
