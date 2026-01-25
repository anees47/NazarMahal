using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Interfaces.IReadModelRepository;
using NazarMahal.Application.Interfaces.IRepository;
using NazarMahal.Infrastructure.Data;
using NazarMahal.Infrastructure.ReadModelRepository;
using NazarMahal.Infrastructure.Repository;
using NazarMahal.Infrastructure.Services;
using System.Data;
using System.Text;

namespace NazarMahal.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });

        _ = services.AddHttpContextAccessor();
        _ = services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    _ = sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                })
        );

        _ = services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        _ = services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(10);
        });

        _ = services.AddScoped<IGlassesRepository, GlassesRepository>();
        _ = services.AddScoped<IGlassesReadModelRepository, GlassesReadModelRepository>();
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IOrderRepository, OrderRepository>();
        _ = services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        _ = services.AddScoped<IEmailService, EmailService>();
        _ = services.AddScoped<INotificationService, NotificationService>();
        _ = services.AddScoped<IAuthService, AuthService>();

        _ = services.AddScoped<Core.Abstractions.IRequestContextAccessor, RequestContextAccessor>();
        _ = services.AddScoped<IFileStorage, FileStorage>(sp => new FileStorage("wwwroot"));

        _ = services.AddScoped<IDbConnection>(sp =>
        {
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();
            return dbContext.Database.GetDbConnection();
        });

        // Validate JWT configuration before setting up authentication
        var jwtKey = configuration["Jwt:Key"];
        var jwtIssuer = configuration["Jwt:Issuer"];
        var jwtAudience = configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException(
                "JWT Key is not configured. Please set 'Jwt:Key' in appsettings.json or environment variables. " +
                "The key must be at least 32 characters long for HS256 algorithm.");
        }

        if (string.IsNullOrWhiteSpace(jwtIssuer))
        {
            throw new InvalidOperationException(
                "JWT Issuer is not configured. Please set 'Jwt:Issuer' in appsettings.json or environment variables.");
        }

        if (string.IsNullOrWhiteSpace(jwtAudience))
        {
            throw new InvalidOperationException(
                "JWT Audience is not configured. Please set 'Jwt:Audience' in appsettings.json or environment variables.");
        }

        // Validate key length (minimum 32 bytes for HS256)
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        if (keyBytes.Length < 32)
        {
            throw new InvalidOperationException(
                $"JWT Key must be at least 32 characters (bytes) long. Current length: {keyBytes.Length}. " +
                "Please generate a secure key using: Convert.ToBase64String(new byte[32]) or use a longer key.");
        }

        _ = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ClockSkew = TimeSpan.Zero
            };
        });

        _ = services.AddAuthorizationBuilder().AddPolicy("RequireAdminUserType", policy =>
                policy.RequireClaim("UserType", "Admin"));

        return services;
    }
}
