using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OnlineStore.Application.Interfaces.Services.Images
{
    public interface IImageStorageService
    {
        Task<string> SaveAsync(IFormFile file);
        Task DeleteAsync(string imageUrl);
    }
}
