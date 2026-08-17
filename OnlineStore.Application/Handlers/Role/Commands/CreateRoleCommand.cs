using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Role.Commands
{
    public sealed record CreateRoleCommand(string RoleName);
}
