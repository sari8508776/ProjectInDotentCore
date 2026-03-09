using myProject.Models;
using Microsoft.AspNetCore.Http;


namespace myProject.Interfaces
{
    public interface IActiveUser
    {
        User ActiveUser { get; }
    }
}