using Microsoft.Extensions.DependencyInjection;
using myProject.Interfaces;
using myProject.Services;

namespace myProject.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            services.AddSingleton<IIceCreamService, IceCreamService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddHttpContextAccessor();
            services.UseActiveUser();
            services.AddSignalR();
            services.AddSingleton<IActivityRepository, ActivityRepository>();
            
            return services;
        }
    }
}