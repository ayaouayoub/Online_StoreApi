
using OnlineStore.Application.Dtos;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.User.Mappings
{
    public static class UserMappings
    {
        public static UserDto ToDto(this Domain.Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                CreatedAt = user.CreatedAt,
                IsActive = user.IsActive,
                Permissions = [.. user.Permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Code = p.Code,
                    Name = p.Name
                })],
                RoleType = (RoleType)user.RoleId,
                Username = user.Username
            };
        }

    }
}
