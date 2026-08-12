using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Customer.Commands
{
    public sealed record RegisterCustomerCommand
    (
        string Name,
        string Username,
        string Password,
        string Email,
        string Address,
        string? Phone
    );
}
