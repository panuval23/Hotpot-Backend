using HotPot23API.Contexts;
using HotPot23API.Models;
using HotPot23API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotPot3API.Repositories
{
    public class UserMasterRepositoryDB : RepositoryDB<int, UserMaster>
    {
        public UserMasterRepositoryDB(FoodDeliveryManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<UserMaster>> GetAll()
        {
            return await _context.UserMasters.ToListAsync();
        }

        public override async Task<UserMaster> GetById(int key)
        {
            var result = await _context.UserMasters
                                       .Include(u => u.AuthUser)
                                       .FirstOrDefaultAsync(u => u.UserID == key);
            return result!;
        }
    }
}

