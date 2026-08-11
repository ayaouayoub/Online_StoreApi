using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using OnlineStore.Application.Interfaces.Services.Images;

namespace OnlineStore.Infrastructure.Services.Storage
{
    public sealed class LocalImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public LocalImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);

            string fileName = $"{Guid.NewGuid()}{extension}";

            string folder = Path.Combine(_environment.WebRootPath, "images", "products");

            Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, fileName);

            using FileStream stream = new(path, FileMode.Create);

            await file.CopyToAsync(stream);

            return $"/images/products/{fileName}";
        }
    }
}
