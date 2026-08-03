using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            User? user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;

            if (user is null)
                return Task.CompletedTask;

            if (user.RoleId == (int)RoleType.SuperAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (user.Permissions.Any(p => p.Code == requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
