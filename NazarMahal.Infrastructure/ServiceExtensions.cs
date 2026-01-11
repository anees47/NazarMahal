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

                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                ),
                ClockSkew = TimeSpan.Zero
            };
        });

        _ = services.AddAuthorizationBuilder().AddPolicy("RequireAdminUserType", policy =>
                policy.RequireClaim("UserType", "Admin"));

        return services;
    }
}
