using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPot23API.Repositories
{
    public class RestaurantMasterRepository : Repository<int, RestaurantMaster>
    {
        public override async Task<RestaurantMaster> GetById(int key)
        {
            var restaurant = list.FirstOrDefault(r => r.RestaurantID == key);
            if (restaurant == null)
                throw new NoSuchEntityException();
            return restaurant;
        }

        
    }
}
