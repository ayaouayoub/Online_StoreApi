using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Dtos
{
    public sealed record RoleDto
    {
        public int RoleId { get; init; }
        public string RoleName { get; init; } = null!;
    }
}
