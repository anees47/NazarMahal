using Microsoft.AspNetCore.Http;
using NazarMahal.Core.Abstractions;
using System.IO;

namespace NazarMahal.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IFileStorage that wraps ASP.NET Core's IFormFile
    /// </summary>
    public class FileStorage : IFileStorage
    {
        private readonly string _basePath;

        public FileStorage(string basePath = "wwwroot")
        {
            _basePath = basePath;
        }

        public async Task<string> SaveFileAsync(IFile file, string directory)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = Path.Combine(directory, fileName).Replace("\\", "/");
            var fullPath = Path.Combine(_basePath, relativePath);

            // Ensure directory exists
            var directoryPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath!);
            }

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.OpenReadStream().CopyToAsync(fileStream);
            }

            return relativePath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_basePath, filePath);
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

    /// <summary>
    /// Adapter that wraps IFormFile to implement IFile
    /// </summary>
    public class FormFileAdapter : IFile
    {
        private readonly IFormFile _formFile;

        public FormFileAdapter(IFormFile formFile)
        {
            _formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
        }

        public string FileName => _formFile.FileName;
        public string ContentType => _formFile.ContentType;
        public long Length => _formFile.Length;
        public Stream OpenReadStream() => _formFile.OpenReadStream();
    }
}

