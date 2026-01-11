using Microsoft.Extensions.DependencyInjection;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Services;

namespace NazarMahal.Application
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            _ = services.AddScoped<IGlassesService, GlassesService>();
            _ = services.AddScoped<IAppointmentService, AppointmentService>();
            _ = services.AddScoped<IUserService, UserService>();
            _ = services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}

