using Microsoft.Extensions.DependencyInjection;
using myProject.Services;

namespace myProject.Services
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers project services in one call: IceCream, User, ActiveUser and SignalR.
        /// </summary>
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            // keep using existing extension points
            services.AddIceCream();
            services.addUserService();
            services.UseActiveUser();

            // SignalR registration (idempotent if called elsewhere)
            services.AddSignalR();

            return services;
        }
    }
}
