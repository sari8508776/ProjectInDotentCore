using Microsoft.AspNetCore.Mvc;
using System;
using myProject.Interfaces;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;

namespace myProject.Services;

public class IceCreamService : IIceCreamService

{
    private  List<IceCream> list;

    public  IceCreamService()
    {
           list = new List<IceCream>
        {
            new IceCream { Id = 1, Name = "Vanilla Dream", IsVegan = false, UserId = 1},
            new IceCream { Id = 2, Name = "Strawberry Bliss", IsVegan = true, UserId = 1},
            new IceCream { Id = 3, Name = "Chocolate Delight", IsVegan = false, UserId = 2},
            new IceCream { Id = 4, Name = "Mint Chocolate Chip", IsVegan = false, UserId = 5},
            new IceCream { Id = 5, Name = "Cookies and Cream", IsVegan = false, UserId = 6}
        };
    }
  

    public List<IceCream> Get()
    {
        return list;
    }

    public IceCream Find(int id)
    {
        return list.FirstOrDefault(p => p.Id == id);

    }

    public IceCream Get(int id) => Find(id);

    public IceCream Create(IceCream newIceCream)
    {
        var maxId = list.Max(p => p.Id);
        newIceCream.Id = maxId + 1;
        list.Add(newIceCream);
        return newIceCream;
    }

    public bool Update(int id, IceCream newIceCream)
    {
        var iceCream = Find(id);
        if (iceCream == null)
            return false;
        if (iceCream.Id != newIceCream.Id)
            return false;

        var index = list.IndexOf(iceCream);
        list[index] = newIceCream;

        return true;
    }

    public bool Delete(int id)
    {
        var iceCream = Find(id);
        if (iceCream == null)
            return false;
        list.Remove(iceCream);
        return true;
    }
      
}

public static class IceCreamExtension
    {
    public   static void AddIceCream(this IServiceCollection services)
        {
            services.AddSingleton<IIceCreamService,IceCreamService>();          
        }
    }

