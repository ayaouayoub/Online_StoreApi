namespace OnlineStore.Api.Controllers.Category.Requests
{
    public sealed record UpdateCategoryRequest
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public int? ParentCategoryId { get; init; }
        public int DisplayOrder { get; init; }
    }
}
