using HotPot23API.Contexts;
using HotPot23API.Models;
using HotPot23API.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPot23API.Repositories
{
    public class MenuItemRepositoryDB : RepositoryDB<int, MenuItemMaster>
    {
        public MenuItemRepositoryDB(FoodDeliveryManagementContext context) : base(context) { }

        public override async Task<MenuItemMaster> GetById(int key)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.Restaurant)
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.MenuItemID == key);

            if (menuItem == null)
                throw new NoSuchEntityException();

            return menuItem;
        }

        public override async Task<IEnumerable<MenuItemMaster>> GetAll()
        {
            return await _context.MenuItems
                .Include(m => m.Restaurant)
                .Include(m => m.Category)
                .ToListAsync();
        }
    }
}
