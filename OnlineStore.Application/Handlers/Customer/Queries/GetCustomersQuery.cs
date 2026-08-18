using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Customer.Queries
{
    public sealed record GetCustomersQuery
    {
        public string? Email { get; init; }
        public string? Phone { get; init; }
        public string? Name { get; init; }
        public string? Username { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
