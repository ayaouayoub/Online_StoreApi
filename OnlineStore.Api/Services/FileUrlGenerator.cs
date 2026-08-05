namespace OnlineStore.Api.Services
{
    public sealed class FileUrlGenerator
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public FileUrlGenerator(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string GetUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;

            HttpRequest request = _contextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}{relativePath}";
        }
    }
}
