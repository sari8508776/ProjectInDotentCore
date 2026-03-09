using myProject.Interfaces;
using myProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace myProject.Services
{
    public class ActiveUserService : IActiveUser
    {
        public User ActiveUser { get; private set; }
        public ActiveUserService(IHttpContextAccessor context)
        {
            var userId = context?.HttpContext?.User?.FindFirst("userid");
            if (userId != null)
            {
                ActiveUser = new User
                {
                    Id = int.Parse(userId.Value),
                    Name = "test",
                    Password = "test",
                    Gender = "test"
                    
                };
            }
        }

    }

    public static class KsPizzaExtensions
    {
        public static IServiceCollection UseActiveUser(this IServiceCollection services)
        {
            services.AddScoped<IActiveUser, ActiveUserService>();
            return services;
        }
    }
}