using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Role.Mappings
{
    public static class RoleMappings
    {
        public static RoleDto ToDto(this Domain.Entities.Role role)
        {
            return new RoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
        }
    }
}
