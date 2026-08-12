using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Order.Commands
{
    public sealed record DeliverOrderCommand(int OrderId);
}
