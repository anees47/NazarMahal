using Microsoft.AspNetCore.Http;

namespace NazarMahal.Application.Interfaces
{
    public interface IFileStorage
    {
        Task<string> SaveFileAsync(IFormFile file, string directory);
        Task DeleteFileAsync(string filePath);
        string GetContentType(string fileName);
    }
}
