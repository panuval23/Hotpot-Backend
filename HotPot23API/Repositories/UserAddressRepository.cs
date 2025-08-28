using HotPot23API.Models;
using HotPot23API.Repositories;

namespace HotPot3API.Repositories
{
    public class UserAddressRepository : Repository<int, UserAddressMaster>
    {
        public override Task<UserAddressMaster?> GetById(int key)
        {
            var address = list.FirstOrDefault(a => a.AddressID == key);
            return Task.FromResult(address);
        }
    }
}
