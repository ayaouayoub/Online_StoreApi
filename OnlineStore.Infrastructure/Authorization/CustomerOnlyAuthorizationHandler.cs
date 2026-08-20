using Microsoft.AspNetCore.Authorization;
using OnlineStore.Application.Interfaces;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Infrastructure.Authorization;

namespace OnlineStore.Application.Authorization
{
    public sealed class CustomerOnlyAuthorizationHandler : AuthorizationHandler<CustomerOnlyRequirement>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUser _currentUser;

        public CustomerOnlyAuthorizationHandler(ICustomerRepository customerRepository, ICurrentUser currentUser)
        {
            _customerRepository = customerRepository;
            _currentUser = currentUser;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomerOnlyRequirement requirement)
        {
            if (_currentUser.UserId <= 0) return;
            var customer = await _customerRepository.GetByUserIdAsync(_currentUser.UserId);
            if (customer is not null) context.Succeed(requirement);
        }
    }
}