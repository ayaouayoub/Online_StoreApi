namespace OnlineStore.Api.Controllers.User.Requests
{
    public sealed record UpdateUserPermissionsRequest
    {
        public IReadOnlyCollection<int> PermissionIds { get; init; } = [];
    }
}
