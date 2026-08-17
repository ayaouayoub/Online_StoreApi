using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Permission.Mappings;
using OnlineStore.Application.Handlers.Permission.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Permission
{
    public sealed class GetPermissionsHandler
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionsHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<IEnumerable<PermissionDto>> ExecuteAsync(GetPermissionsQuery query)
        {
            var permission = await _permissionRepository.GetAllAsync();
            return [.. permission.Select(p => p.ToDto())];
        }
    }
}
