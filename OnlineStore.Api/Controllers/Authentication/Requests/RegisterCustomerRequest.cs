namespace OnlineStore.Api.Controllers.Authentication.Requests
{
    public sealed record RegisterCustomerRequest
    {
        public string Name { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Address { get; init; } = null!;
        public string? Phone { get; init; }
    }
}
