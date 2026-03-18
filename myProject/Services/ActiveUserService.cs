using myProject.Interfaces;
using myProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;


namespace myProject.Services
{
    public class ActiveUserService : IActivUserService
    {
        public User ActiveUser { get; private set; }
        public ActiveUserService(IHttpContextAccessor context)
        {
            var user = context?.HttpContext?.User;
            if (user == null)
                return;

            // try several common claim names to be compatible with different token issuers
            var idClaim = user.FindFirst("userid") ?? user.FindFirst("Id") ?? user.FindFirst("sub");
            if (idClaim == null)
                return;

            var nameClaim = user.FindFirst("username") ?? user.FindFirst("name");
            var genderClaim = user.FindFirst("gender");

            // create a minimal User object from available claims. Model requires non-null strings, so default to empty string when missing.
            ActiveUser = new User
            {
                Id = int.TryParse(idClaim.Value, out var parsed) ? parsed : 0,
                Name = nameClaim?.Value ?? string.Empty,
                Password = string.Empty,
                Gender = genderClaim?.Value ?? string.Empty
            };
        }

    }

    public static class ActiveUserExtensions
    {
        public static IServiceCollection UseActiveUser(this IServiceCollection services)
        {
            services.AddScoped<IActivUserService, ActiveUserService>();
            return services;
        }
    }
}