using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPot23API.Repositories
{
    public class RestaurantMasterRepositoryDB : RepositoryDB<int, RestaurantMaster>
    {
        public RestaurantMasterRepositoryDB(FoodDeliveryManagementContext context) : base(context) { }

        public override async Task<RestaurantMaster> GetById(int key)
        {
            var restaurant = await _context.RestaurantMasters
                .Include(r => r.User)
                .Include(r => r.MenuItems)
                .Include(r => r.Categories)
                .Include(r => r.Orders)
                .FirstOrDefaultAsync(r => r.RestaurantID == key);

            if (restaurant == null)
                throw new NoSuchEntityException();

            return restaurant;
        }

        public override async Task<IEnumerable<RestaurantMaster>> GetAll()
        {
            return await _context.RestaurantMasters
                .Include(r => r.User)
                .Include(r => r.MenuItems)
                .Include(r => r.Categories)
                .Include(r => r.Orders)
                .ToListAsync();
        }
    }
}
