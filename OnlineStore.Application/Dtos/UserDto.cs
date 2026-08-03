using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Dtos
{
    public class UserDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Username { get; init; } = null!;
        public RoleType RoleType { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
        public List<PermissionDto> Permissions { get; init; } = [];
    }
}
