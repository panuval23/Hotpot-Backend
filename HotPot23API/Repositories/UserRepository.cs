using HotPot23API.Contexts;
using HotPot23API.Models;
using HotPot23API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotPot23API.Repositories
{
    public class UserRepository : RepositoryDB<string, User>
    {
        public UserRepository(FoodDeliveryManagementContext context):base(context)
        {
            
        }
        public async override Task<IEnumerable<User>> GetAll()
        {
            return _context.Users;
        }

        public async override Task<User> GetById(string key)
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Username == key);
            return result;
        }
    }
}
