using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Handlers.Order.Commands
{
    public sealed record PayOrderCommand(int OrderId, int PaymentMethodId, PaymentProvider Provider);
}
