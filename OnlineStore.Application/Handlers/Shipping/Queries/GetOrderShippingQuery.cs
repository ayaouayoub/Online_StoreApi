using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Shipping.Queries
{
    public sealed record GetOrderShippingQuery(int OrderId);
}
