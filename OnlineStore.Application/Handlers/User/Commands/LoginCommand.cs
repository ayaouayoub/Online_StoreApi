using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.User.Commands
{
    public record LoginCommand(string Username, string Password);
}
