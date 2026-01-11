using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using NazarMahal.API.Middleware;
using NazarMahal.Application;
using NazarMahal.Infrastructure;
using System.IO.Compression;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.Configure<HostFilteringOptions>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Allow all hosts in development
        options.AllowedHosts.Clear();
        options.AllowedHosts.Add("*");
    }
    else
    {
        // Restrict to production domains only
        options.AllowedHosts.Clear();
        options.AllowedHosts.Add("nazarmahal.com");
        options.AllowedHosts.Add("www.nazarmahal.com");
        options.AllowedHosts.Add("api.nazarmahal.com");
    }
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development: Allow localhost
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            _ = policy.WithOrigins(
                    "http://localhost:4200",
                    "https://localhost:4200"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    }
    else
    {
        // Production: Only allow production domains
        options.AddPolicy("AllowSpecificOrigins", policy =>
        {
            _ = policy.WithOrigins(
                    "https://nazarmahal.com",
                    "https://www.nazarmahal.com"
                )
                .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
                .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
                .AllowCredentials();
        });
    }
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NazarMahal API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddRateLimiter(options =>
{
    // Global rate limiter: 100 requests per minute per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 50,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Specific policy for authentication endpoints: 5 requests per minute
    _ = options.AddPolicy("AuthPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1)
            }));

    // Rejection response
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync(
            "Rate limit exceeded. Please try again later.", cancellationToken: token);
    };
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json", "text/json", "application/problem+json" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
    options.ValueLengthLimit = 10485760; // 10 MB
});


var app = builder.Build();

// Configuration Validation
if (!app.Environment.IsDevelopment())
{
    _ = app.UseMiddleware<ConfigurationValidationMiddleware>();
}

// Global Exception Handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Security Headers
app.UseMiddleware<SecurityHeadersMiddleware>();

// Swagger
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// Host Filtering
app.UseHostFiltering();

// HTTPS Redirection & HSTS (Production)
if (!app.Environment.IsDevelopment())
{
    _ = app.UseHsts(); // HTTP Strict Transport Security
    _ = app.UseHttpsRedirection();
}
else
{
    _ = app.UseHttpsRedirection();
}

// Response Compression
app.UseResponseCompression();

// Static Files
app.UseStaticFiles();

// CORS
app.UseCors("AllowSpecificOrigins");

// Rate Limiting
app.UseRateLimiter();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

app.Run();
