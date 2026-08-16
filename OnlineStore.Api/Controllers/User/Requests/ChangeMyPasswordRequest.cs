namespace OnlineStore.Api.Controllers.User.Requests
{
    public sealed record ChangeMyPasswordRequest(string CurrentPassword, string NewPassword);
}
