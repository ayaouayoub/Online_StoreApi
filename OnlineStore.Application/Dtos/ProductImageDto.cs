namespace OnlineStore.Application.Dtos
{
    public sealed record ProductImageDto
    {
        public string Url { get; set; } = null!;
        public short ImageOrder { get; set; }
    }
}
