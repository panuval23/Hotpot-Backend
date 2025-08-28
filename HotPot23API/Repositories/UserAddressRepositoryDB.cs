using HotPot23API.Contexts;
using HotPot23API.Models;
using HotPot23API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotPot3API.Repositories
{
    public class UserAddressRepositoryDB : RepositoryDB<int, UserAddressMaster>
    {
        public UserAddressRepositoryDB(FoodDeliveryManagementContext context) : base(context)
        {
        }

        public override async Task<UserAddressMaster?> GetById(int key)
        {
            return await _context.UserAddressMasters.FirstOrDefaultAsync(u => u.AddressID == key);
        }

        public override async Task<IEnumerable<UserAddressMaster>> GetAll()
        {
            return await _context.UserAddressMasters.ToListAsync();
        }

        public  async Task<UserAddressMaster> Add(UserAddressMaster item)
        {
            _context.UserAddressMasters.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<UserAddressMaster?> Update(UserAddressMaster item)
        {
            var existing = await GetById(item.AddressID);
            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return existing;
        }

        public  async Task<UserAddressMaster?> Delete(int key)
        {
            var item = await GetById(key);
            if (item == null) return null;

            _context.UserAddressMasters.Remove(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
