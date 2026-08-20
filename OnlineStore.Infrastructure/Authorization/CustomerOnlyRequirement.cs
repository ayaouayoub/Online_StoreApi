using Microsoft.AspNetCore.Authorization;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class CustomerOnlyRequirement : IAuthorizationRequirement;
}
