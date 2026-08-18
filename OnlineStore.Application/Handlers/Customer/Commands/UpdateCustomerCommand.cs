using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Customer.Commands
{
    public sealed record UpdateCustomerCommand
    {
        public int CustomerId { get; init; }

        public string Email { get; init; } = null!;

        public string? Phone { get; init; }

        public string Address { get; init; } = null!;
    }
}
