namespace NazarMahal.Core.Abstractions
{
    /// <summary>
    /// Abstraction for accessing HTTP context information
    /// </summary>
    public interface IRequestContextAccessor
    {
        /// <summary>
        /// Gets the request scheme (http/https)
        /// </summary>
        string RequestScheme { get; }

        /// <summary>
        /// Gets the request host
        /// </summary>
        string RequestHost { get; }

        /// <summary>
        /// Builds a URL from the given path
        /// </summary>
        string BuildUrl(string path);
    }
}

