
// using System.Linq;
using System.Collections.Generic;
// using myProject.Models;


namespace myProject.Interfaces;

public interface IUserService
{

     List<User> Get();

     User find(int id);

     User Get(int id);

     User Create(User newUser);

     bool Update(int id, User newUser);

     bool Delete(int id);

     User Login(string name, string password);
}