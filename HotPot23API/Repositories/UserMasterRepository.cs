using HotPot23API.Exceptions;
using HotPot23API.Models;
using HotPot23API.Repositories;

namespace HotPot3API.Repositories
{
    public class UserMasterRepository : Repository<int, UserMaster>
    {
        public async override Task<UserMaster> GetById(int key)
        {
            var item = list.FirstOrDefault(x => x.UserID == key);
            if (item == null)
                throw new NoSuchEntityException();
            return item;
        }
    }
}
