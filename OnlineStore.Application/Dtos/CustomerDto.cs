using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.ValueObjs;

namespace OnlineStore.Application.Dtos
{
    public sealed record CustomerDto
    {
        public int Id { get; init; }
        public string Email { get; init; } = null!;
        public string? Phone { get; init; }
        public string Address { get; init; } = null!;
        public UserSummaryDto userSummaryDto { get; init; } = null!;
    }
}
