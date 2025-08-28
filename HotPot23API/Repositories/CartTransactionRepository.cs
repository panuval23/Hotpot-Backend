using HotPot23API.Exceptions;
using HotPot23API.Models;

namespace HotPot23API.Repositories
{
    
        public class CartTransactionRepository : Repository<int, CartTransaction>
        {
            public async override Task<CartTransaction> GetById(int key)
            {
                var item = list.FirstOrDefault(x => x.CartID == key);
                if (item == null)
                    throw new NoSuchEntityException();
                return item;
            }
        }
    
}
