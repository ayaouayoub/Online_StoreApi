using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Api.Controllers.Authentication.Requests
{
    public record RefreshRequest
    {
        public string RefreshToken { get; init; } = null!;
        public string Username { get; init; } = null!;
    }
}
