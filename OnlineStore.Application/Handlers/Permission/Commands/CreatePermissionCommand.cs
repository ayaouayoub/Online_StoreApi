namespace OnlineStore.Application.Handlers.Permission.Commands
{
    public sealed record CreatePermissionCommand
    {
        public string Code { get; init; } = null!;
        public string Name { get; init; } = null!;
    }
}
