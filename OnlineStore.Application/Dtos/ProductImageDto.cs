namespace OnlineStore.Application.Dtos
{
    public sealed record ProductImageDto
    {
        public string Url { get; set; } = null!;
        public int ImageOrder { get; set; }
    }
}
