using Microsoft.AspNetCore.Authorization;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class SuperAdminAuthorizationHandler : AuthorizationHandler<SuperAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminRequirement requirement)
        {
            if (context.User.IsInRole(nameof(RoleType.SuperAdmin))) context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
