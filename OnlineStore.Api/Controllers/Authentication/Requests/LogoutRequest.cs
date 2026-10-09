namespace OnlineStore.Api.Controllers.Authentication.Requests
{
    public record LogoutRequest
    {
        public string RefreshToken { get; init; } = null!;
        public string Username { get; init; } = null!;
    }
}
