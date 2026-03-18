using myProject.Models;
using Microsoft.AspNetCore.Http;
namespace myProject.Interfaces
{
    public interface IActivUserService
{
         User ActiveUser { get; }
}
}


