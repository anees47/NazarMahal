using Microsoft.AspNetCore.Http;
using NazarMahal.Application.Interfaces;

namespace NazarMahal.Infrastructure.Services
{
    public class FileStorage(string basePath = "wwwroot") : IFileStorage
    {
        public async Task<string> SaveFileAsync(IFormFile file, string directory)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = Path.Combine(directory, fileName).Replace("\\", "/");
            var fullPath = Path.Combine(basePath, relativePath);

            var directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath!);
            }

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.OpenReadStream().CopyToAsync(fileStream);
            }

            return relativePath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return Task.CompletedTask;

            var fullPath = Path.Combine(basePath, filePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }

        public string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}

