
using Microsoft.AspNetCore.Mvc;
using System;
using myProject.Interfaces;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

namespace myProject;

public class UserService : IUserService
{
    private List<User> Users;
    private string filePath;
    
    private List<User> GetDefaultUsers()
    {
        return new List<User>
        {
            new User { Id = 1, Name = "sari Rabinovitch", Age = 20, Gender = "female", Password = "password1"},
            new User { Id = 2, Name = "Tamer Rotan", Age = 21, Gender = "female", Password = "password2"},
            new User { Id = 4, Name = "Yahakov Cohen", Age = 13, Gender = "male", Password = "password3"},
            new User { Id = 3, Name = "Beni Levi", Age = 23, Gender = "male", Password = "password4"},
            new User { Id = 5, Name = "admin", Age = 25, Gender = "male", Password = "admin"},
            new User { Id = 6, Name = "user", Age = 22, Gender = "female", Password = "user"}
        };
    }

    public UserService(IWebHostEnvironment webHost)
    {
        this.filePath = Path.Combine(webHost.ContentRootPath, "Data", "User.json");
        
        // תמיד התחל עם ברירות המחדל
        Users = GetDefaultUsers();
        
        // אם יש קובץ עם נתונים, נטען אותו
        if (File.Exists(filePath))
        {
            try
            {
                using (var jsonFile = File.OpenText(filePath))
                {
                    var content = jsonFile.ReadToEnd();
                    if (!string.IsNullOrWhiteSpace(content) && content != "[]")
                    {
                        var loadedUsers = JsonSerializer.Deserialize<List<User>>(content,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (loadedUsers != null && loadedUsers.Count > 0)
                        {
                            Users = loadedUsers;
                        }
                    }
                }
            }
            catch
            {
                // אם יש שגיאה בטעינה, נשתמש בברירות המחדל
                Users = GetDefaultUsers();
            }
        }
        
        // תמיד שמור את הנתונים כך שיהיו מעודכנים
        saveToFile();
    }

    private void saveToFile()
    {
        var text = JsonSerializer.Serialize(Users);
        File.WriteAllText(filePath, text);
    }

    public List<User> Get()
    {
        saveToFile();
        return Users;
    }

    public User find(int id)
    {
        return Users.FirstOrDefault(p => p.Id == id);
    }

    public User Get(int id) => find(id);

    public User Create(User newUser)
    {
        var maxId = Users.Max(p => p.Id);
        newUser.Id = maxId + 1;
        Users.Add(newUser);
        saveToFile();

        return newUser;
    }

    public bool Update(int id, User newUser)
    {
        var user = find(id);
        if (user == null || user.Id != newUser.Id)
            return false;

        var index = Users.IndexOf(user);
        Users[index] = newUser;
        saveToFile();

        return true;
    }

    public bool Delete(int id)
    {
        var user = find(id);
        if (user == null)
            return false;

        Users.Remove(user);
        saveToFile();

        return true;
    }

    public User Login(string name, string password)
    {
        return Users.FirstOrDefault(u => u.Name == name && u.Password == password);
    }
}

public static class UserServiceExtension
{
    public static void addUserService(this IServiceCollection services)
    {
        services.AddSingleton<IUserService, UserService>();
    }
}