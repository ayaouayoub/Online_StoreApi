namespace OnlineStore.Api.Controllers.User.Requests
{
    public sealed record CreateUserRequest
    (
        string Name,
        string Username,
        string Password,
        IReadOnlyCollection<int> PermissionIds
    );
}
