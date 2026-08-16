using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Handlers.User.Commands
{
    public sealed class UpdateUserCommand
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
    }
}
