using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Application.Security;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class CustomerViewAuthorizationHandler : AuthorizationHandler<CustomerViewRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerRepository _customerRepository;

        public CustomerViewAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ICustomerRepository customerRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _customerRepository = customerRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerViewRequirement requirement)
        {
            User? user = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;

            if (user is null || !user.IsActive) return;

            if (user.RoleId == (int)RoleType.SuperAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            if (user.Permissions.Any(p => p.Code == Permissions.Customers.View))
            {
                context.Succeed(requirement);
                return;
            }

            if (user.RoleId == (int)RoleType.Customer)
            {
                var result = await _customerRepository.GetByUserIdAsync(user.Id);

                if (result is null) return;

                var requestedCustomerId = _httpContextAccessor.HttpContext!.Request.RouteValues["id"];

                if (!int.TryParse(requestedCustomerId?.ToString(), out int customerId)) return;

                if (result.Customer.Id == customerId) context.Succeed(requirement);
            }
        }
    }
}
