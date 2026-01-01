using Microsoft.AspNetCore.Http;
using NazarMahal.Core.Abstractions;

namespace NazarMahal.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IRequestContextAccessor that wraps ASP.NET Core's HttpContext
    /// </summary>
    public class RequestContextAccessor : IRequestContextAccessor
    {
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public RequestContextAccessor(Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string RequestScheme
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Request.Scheme ?? "https";
            }
        }

        public string RequestHost
        {
            get
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Request.Host.ToString() ?? string.Empty;
            }
        }

        public string BuildUrl(string path)
        {
            var scheme = RequestScheme;
            var host = RequestHost;
            if (string.IsNullOrEmpty(host))
                return path;

            // Remove leading slash from path if present
            if (path.StartsWith("/"))
                path = path.Substring(1);

            return $"{scheme}://{host}/{path}";
        }
    }
}

