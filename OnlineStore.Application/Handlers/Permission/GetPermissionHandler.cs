using OnlineStore.Application.Dtos;
using OnlineStore.Application.Exceptions;
using OnlineStore.Application.Handlers.Permission.Mappings;
using OnlineStore.Application.Handlers.Permission.Queries;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Permission
{
    public sealed class GetPermissionHandler
    {
        private readonly IPermissionRepository _permissionRepository;

        public GetPermissionHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<PermissionDto> ExecuteAsync(GetPermissionQuery query)
        {
            var permission = await _permissionRepository.GetByIdAsync(query.PermissionId) ?? throw new NotFoundException("Permission not found");
            return permission.ToDto();
        }
    }
}
