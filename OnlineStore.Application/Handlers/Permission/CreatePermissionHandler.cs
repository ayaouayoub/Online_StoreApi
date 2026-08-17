using OnlineStore.Application.Dtos;
using OnlineStore.Application.Handlers.Permission.Commands;
using OnlineStore.Application.Handlers.Permission.Mappings;
using OnlineStore.Application.Interfaces.Repositories;

namespace OnlineStore.Application.Handlers.Permission
{
    public sealed class CreatePermissionHandler
    {
        private readonly IPermissionRepository _permissionRepository;

        public CreatePermissionHandler(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }
        public async Task<PermissionDto> ExecuteAsync(CreatePermissionCommand command)
        {
            var permission = Domain.Entities.Permission.Create(command.Code, command.Name);
            var createdPermission = await _permissionRepository.CreateAsync(permission);
            return createdPermission.ToDto();
        }
    }
}
