using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Order.Queries
{
    public sealed record GetOrdersQuery
    {
        public int? CustomerId { get; init; }
        public OrderStatus? Status { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
