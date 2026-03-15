using Microsoft.Extensions.DependencyInjection;
using myProject.Services;

namespace myProject.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            // keep using existing extension points
            services.AddIceCream();
            services.addUserService();
            services.UseActiveUser();
            services.AddSignalR();
            services.AddSingleton<IActivityRepository, ActivityRepository>();
            return services;
        }
    }
}
