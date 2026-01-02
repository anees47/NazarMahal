using Microsoft.Extensions.DependencyInjection;
using NazarMahal.Application.Interfaces;
using NazarMahal.Application.Services;

namespace NazarMahal.Application
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IGlassesService, GlassesService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}

