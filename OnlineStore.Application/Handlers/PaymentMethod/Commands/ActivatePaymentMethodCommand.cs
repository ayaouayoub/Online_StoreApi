using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.PaymentMethod.Commands
{
    public sealed record ActivatePaymentMethodCommand(int PaymentMethodId);
}
