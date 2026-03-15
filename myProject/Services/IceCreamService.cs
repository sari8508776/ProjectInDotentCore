using Microsoft.AspNetCore.Mvc;
using System;
using myProject.Interfaces;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using myProject.Hubs;
using Microsoft.AspNetCore.Http;

namespace myProject.Services;

public class IceCreamService : IIceCreamService

{
    private List<IceCream> list;
    private string filePath;

 private List<IceCream> GetDefaultIceCreams(){
    return new List<IceCream>
    
    {
            new IceCream { Id = 1, Name = "Vanilla Dream", IsVegan = false, UserId = 1},
            new IceCream { Id = 2, Name = "Strawberry Bliss", IsVegan = true, UserId = 1},
            new IceCream { Id = 3, Name = "Chocolate Delight", IsVegan = false, UserId = 2},
            new IceCream { Id = 4, Name = "Mint Chocolate Chip", IsVegan = false, UserId = 5},
            new IceCream { Id = 5, Name = "Cookies and Cream", IsVegan = false, UserId = 6}
        };
 }
 
    public IceCreamService(IWebHostEnvironment webHost)
    {
       this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "icecreams.json");

        list = GetDefaultIceCreams();

        if (File.Exists(filePath))
        {
            try
            {
                using (var jsonFile = File.OpenText(filePath))
                {
                    var content = jsonFile.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(content) && content != "[]")
                    {
                        var loadedUsers = JsonSerializer.Deserialize<List<IceCream>>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (loadedUsers != null && loadedUsers.Count > 0)
                        {
                            list = loadedUsers;
                        }
                    }
                }
            }
            catch
            {
                // אם יש שגיאה בטעינה, נשתמש בברירות המחדל
                list = GetDefaultIceCreams();
            }
        }

        // תמיד שמור את הנתונים כך שיהיו מעודכנים
        saveToFile();
    

        
    }

    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(list);
        File.WriteAllText(filePath, text);
    }


    public List<IceCream> Get()
    {
        saveToFile();
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
       saveToFile();
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
        saveToFile();
        return true;
    }

    public bool Delete(int id)
    {
        var iceCream = Find(id);
        if (iceCream == null)
            return false;
        list.Remove(iceCream);
        saveToFile();
        return true;
    }

   

}

public static class IceCreamExtension
{
    public static void AddIceCream(this IServiceCollection services)
    {
        services.AddSingleton<IIceCreamService, IceCreamService>();
    }
}

