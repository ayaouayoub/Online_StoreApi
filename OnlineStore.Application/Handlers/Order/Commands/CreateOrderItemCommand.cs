using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Order.Commands
{
    public sealed record CreateOrderItemCommand
    {
        public int ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
