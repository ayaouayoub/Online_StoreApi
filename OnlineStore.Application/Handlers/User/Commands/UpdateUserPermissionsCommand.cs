namespace OnlineStore.Application.Handlers.User.Commands
{
    public sealed record UpdateUserPermissionsCommand(int UserId, IReadOnlyCollection<int> PermissionIds);
}
