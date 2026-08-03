using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Interfaces.Repositories;
using OnlineStore.Domain.Entities;

namespace OnlineStore.Infrastructure.Authorization
{
    public sealed class ActiveUserAuthorizationHandler : AuthorizationHandler<ActiveUserRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public ActiveUserAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ActiveUserRequirement requirement)
        {
            string? id = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(id, out int userId))
                return;

            User? user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                return;

            if (!user.IsActive)
                return;

            _httpContextAccessor.HttpContext!.Items["CurrentUser"] = user;

            context.Succeed(requirement);
        }
    }
}
