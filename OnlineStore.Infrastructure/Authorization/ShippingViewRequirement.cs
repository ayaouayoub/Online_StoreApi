using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class ShippingViewRequirement : IAuthorizationRequirement { }
}
