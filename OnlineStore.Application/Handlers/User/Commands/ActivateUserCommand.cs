using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.User.Commands
{
    public sealed record ActivateUserCommand(int UserId);
}
