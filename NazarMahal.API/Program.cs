using Microsoft.AspNetCore.HostFiltering;
using Microsoft.OpenApi.Models;
using NazarMahal.Application;
using NazarMahal.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

/* ================================
 * Kestrel
 * ================================ */
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
});

/* ================================
 * Host Filtering (Dev only)
 * ================================ */
builder.Services.Configure<HostFilteringOptions>(options =>
{
    options.AllowedHosts.Clear();
    options.AllowedHosts.Add("*");
});

/* ================================
 * Controllers
 * ================================ */
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            JsonIgnoreCondition.WhenWritingNull;
    });

/* ================================
 * Application Layers
 * ================================ */
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

/* ================================
 * CORS
 * ================================ */
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "https://dev.nazarmahal.com",
                "https://api-dev.nazarmahal.com",
                "https://nazarmahal.com",
                "https://www.nazarmahal.com",
                "https://api.nazarmahal.com"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

/* ================================
 * Swagger
 * ================================ */
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

var app = builder.Build();

/* ================================
 * Middleware Pipeline
 * ================================ */
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHostFiltering();
app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
