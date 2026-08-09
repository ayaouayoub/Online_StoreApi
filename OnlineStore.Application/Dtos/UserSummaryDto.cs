using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Dtos
{
    public sealed record UserSummaryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string Username { get; init; } = null!;
        public bool IsActive { get; init; }
    }
}
