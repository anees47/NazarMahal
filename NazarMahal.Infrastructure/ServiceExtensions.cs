using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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
        services.ConfigureApplicationCookie(options =>
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

        services.AddHttpContextAccessor();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                })
        );

        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(10);
        });

        services.AddScoped<IGlassesRepository, GlassesRepository>();
        services.AddScoped<IGlassesReadModelRepository, GlassesReadModelRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<Core.Abstractions.IRequestContextAccessor, RequestContextAccessor>();
        services.AddScoped<IFileStorage, FileStorage>(sp => new FileStorage("wwwroot"));

        // IDbConnection factory that uses the pooled connection from DbContext
        // This ensures Dapper uses the same connection pool as EF Core
        services.AddScoped<IDbConnection>(sp =>
        {
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();
            return dbContext.Database.GetDbConnection();
        });

        services.AddAuthentication(options =>
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

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                ),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorizationBuilder().AddPolicy("RequireAdminUserType", policy =>
                policy.RequireClaim("UserType", "Admin"));

        return services;
    }
}
