using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.User.Queries
{
    public sealed record GetUsersQuery
    {
        public string? Name { get; init; }
        public string? Username { get; init; }
        public int? RoleId { get; init; }
        public bool? IsActive { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
