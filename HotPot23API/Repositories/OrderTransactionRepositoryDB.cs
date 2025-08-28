using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPot23API.Repositories
{
    public class OrderTransactionRepositoryDB : RepositoryDB<int, OrderTransaction>
    {
        public OrderTransactionRepositoryDB(FoodDeliveryManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<OrderTransaction>> GetAll()
        {
            return await _context.OrderTransactions
                                 .Include(o => o.User)
                                 .Include(o => o.Restaurant)
                                 .Include(o => o.ShippingAddress)
                                 .Include(o => o.OrderItems)
                                     .ThenInclude(oi => oi.MenuItem)
                                 .Include(o => o.Payment)
                                 .Include(o => o.DeliveryStatuses)
                                 .ToListAsync();
        }

        public override async Task<OrderTransaction> GetById(int key)
        {
            var result = await _context.OrderTransactions
                                       .Include(o => o.User)
                                       .Include(o => o.Restaurant)
                                       .Include(o => o.ShippingAddress)
                                       .Include(o => o.OrderItems)
                                           .ThenInclude(oi => oi.MenuItem)
                                       .Include(o => o.Payment)
                                       .Include(o => o.DeliveryStatuses)
                                       .FirstOrDefaultAsync(o => o.OrderID == key);

            if (result == null)
                throw new NoSuchEntityException();

            return result;
        }
    }
}
