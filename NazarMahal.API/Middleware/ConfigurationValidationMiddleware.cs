using Microsoft.Extensions.Configuration;

namespace NazarMahal.API.Middleware;

public class ConfigurationValidationMiddleware(RequestDelegate next, ILogger<ConfigurationValidationMiddleware> logger, IConfiguration configuration, IWebHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate in production
        if (!environment.IsDevelopment())
        {
            var jwtKey = configuration["Jwt:Key"];
            var brevoApiKey = configuration["BrevoSettings:ApiKey"];
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var missingSecrets = new List<string>();

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                missingSecrets.Add("Jwt:Key");
            }

            if (string.IsNullOrWhiteSpace(brevoApiKey))
            {
                missingSecrets.Add("BrevoSettings:ApiKey");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                missingSecrets.Add("ConnectionStrings:DefaultConnection");
            }

            if (missingSecrets.Any())
            {
                var keyVaultUrl = configuration["KeyVault:Url"];
                var errorMessage = string.IsNullOrWhiteSpace(keyVaultUrl)
                    ? "Required configuration values are missing. Please check environment variables or Azure Key Vault."
                    : $"Required configuration values are missing. Please check Azure Key Vault ({keyVaultUrl}) or environment variables.";
                
                logger.LogCritical("CRITICAL: Missing required configuration secrets: {Secrets}. KeyVault:Url: {KeyVaultUrl}", 
                    string.Join(", ", missingSecrets), keyVaultUrl ?? "Not configured");
                
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "Configuration error",
                    message = errorMessage
                }));
                return;
            }
        }

        await next(context);
    }
}
