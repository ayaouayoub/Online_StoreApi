using OnlineStore.Application.Dtos;

namespace OnlineStore.Api.Controllers.User.Requests
{
    public sealed record UpdateUserRequest
    {
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
