namespace NazarMahal.Core.Abstractions
{
    /// <summary>
    /// Abstraction for file storage operations
    /// </summary>
    public interface IFileStorage
    {
        /// <summary>
        /// Saves a file and returns the relative path
        /// </summary>
        Task<string> SaveFileAsync(IFile file, string directory);

        /// <summary>
        /// Deletes a file by its path
        /// </summary>
        Task DeleteFileAsync(string filePath);

        /// <summary>
        /// Gets the file content type
        /// </summary>
        string GetContentType(string fileName);
    }

    /// <summary>
    /// Abstraction for file data
    /// </summary>
    public interface IFile
    {
        string FileName { get; }
        string ContentType { get; }
        long Length { get; }
        Stream OpenReadStream();
    }
}

