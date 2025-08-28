using HotPot23API.Exceptions;
using HotPot23API.Models;

namespace HotPot23API.Repositories
{
    public class OrderTransactionRepository : Repository<int, OrderTransaction>
    {
        public async override Task<OrderTransaction> GetById(int key)
        {
            var item = list.FirstOrDefault(x => x.OrderID == key);
            if (item == null)
                throw new NoSuchEntityException();
            return item;
        }
    }
}
