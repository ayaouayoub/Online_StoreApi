using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Application.Dtos;

namespace OnlineStore.Application.Handlers.Order.Commands
{
    public sealed record CreateOrderCommand
    {
        public IReadOnlyCollection<CreateOrderItemCommand> Items { get; init; } = [];
    }
}
