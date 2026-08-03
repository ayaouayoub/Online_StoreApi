using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Interfaces;
using OnlineStore.Domain.Entities;
using OnlineStore.Domain.Enums;

namespace OnlineStore.Infrastructure.Authentication
{
    public sealed class CurrentUserAccessor : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentUserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public User User => (User)_accessor.HttpContext!.Items["CurrentUser"]!;

        public int UserId => User.Id;

        public string Name => User.Name;

        public string Username => User.Username;

        public bool IsActive => User.IsActive;

        public IReadOnlyCollection<string> Permissions => User.Permissions.Select(p => p.Code).ToList();

        public RoleType RoleType => (RoleType)User.RoleId;
    }
}
