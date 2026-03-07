
using System.Collections.Generic;

namespace myProject.Interfaces
{
    public interface IIceCreamService
    {
        bool Delete(int id);
        bool Update(int id, IceCream newIceCream);
        IceCream Create(IceCream newIceCream);
        IceCream Find(int id);
        List<IceCream> Get();
        IceCream Get(int id);

    }
}
