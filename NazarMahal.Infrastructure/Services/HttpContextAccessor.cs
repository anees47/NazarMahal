using Microsoft.AspNetCore.Http;
using NazarMahal.Core.Abstractions;

namespace NazarMahal.Infrastructure.Services;

public class RequestContextAccessor(IHttpContextAccessor httpContextAccessor) : IRequestContextAccessor
{
    public string RequestScheme
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            return httpContext?.Request.Scheme ?? "https";
        }
    }

    public string RequestHost
    {
        get
        {
            var httpContext = httpContextAccessor.HttpContext;
            return httpContext?.Request.Host.ToString() ?? string.Empty;
        }
    }

    public string BuildUrl(string path)
    {
        var scheme = RequestScheme;
        var host = RequestHost;
        if (string.IsNullOrEmpty(host))
            return path;

        if (path.StartsWith("/"))
            path = path.Substring(1);

        return $"{scheme}://{host}/{path}";
    }
}

