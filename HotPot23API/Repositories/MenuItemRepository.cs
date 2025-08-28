using HotPot23API.Models;
using HotPot23API.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPot23API.Repositories
{
    public class MenuItemRepository : Repository<int, MenuItemMaster>
    {
        public override async Task<MenuItemMaster> GetById(int key)
        {
            var item = list.FirstOrDefault(m => m.MenuItemID == key);
            if (item == null)
                throw new NoSuchEntityException();
            return item;
        }

       
    }
}
