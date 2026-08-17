using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Permission.Mappings
{
    public static class PermisssionMappings
    {
        public static PermissionDto ToDto(this Domain.Entities.Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Code = permission.Code,
                Name = permission.Name,
            };
        }
    }
}
