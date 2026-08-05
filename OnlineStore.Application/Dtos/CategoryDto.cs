namespace OnlineStore.Application.Dtos
{
    public class CategoryDto
    {
        public int? Id { get; init; }
        public string? Name { get; init; } = null!;
        public string? Description { get; init; }
        public int? DisplayOrder { get; init; }
        public bool? IsActive { get; init; }
        public DateTime? CreatedAt { get; init; }
    }
}
