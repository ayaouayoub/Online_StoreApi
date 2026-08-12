using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OnlineStore.Application.Security;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class ShippingViewAuthorizationHandler : AuthorizationHandler<ShippingViewRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ShippingViewRequirement requirement)
        {
            if (context.User.IsInRole(nameof(RoleType.Customer)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (context.User.IsInRole(nameof(RoleType.SuperAdmin)))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var hasPermission = context.User.Claims.Any(c => c.Type == "permission" && c.Value == Permissions.Shipping.View);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
