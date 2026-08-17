using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Shipping.Queries
{
    public sealed record GetShippingsQuery
    {
        public ShippingStatus? ShippingStatus { get; init; }
        public string? CarrierName { get; init; }
        public string? TrackingNumber { get; init; }
        public int? OrderId { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
