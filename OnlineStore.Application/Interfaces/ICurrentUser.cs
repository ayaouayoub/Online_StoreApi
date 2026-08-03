using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Application.Interfaces
{
    public interface ICurrentUser
    {
        User User { get; }

        int UserId { get; }

        string Username { get; }

        RoleType RoleType { get; }

        bool IsActive { get; }

        IReadOnlyCollection<string> Permissions { get; }
    }
}
