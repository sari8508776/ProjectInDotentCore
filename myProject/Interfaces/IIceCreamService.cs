
using System.Collections.Generic;

namespace myProject.Interfaces
{
    public interface IIceCreamService
    {
        bool Delete(int id);
       
        void DeleteByUserId(int userId);
        bool Update(int id, IceCream newIceCream);
        IceCream Create(IceCream newIceCream);
        IceCream Find(int id);
        List<IceCream> Get();
        IceCream Get(int id);

    }
}
