using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Models;
using HotPot23API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HotPot23API.Repositories
{
    public class CartTransactionRepositoryDB : RepositoryDB<int, CartTransaction>
    {
        public CartTransactionRepositoryDB(FoodDeliveryManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<CartTransaction>> GetAll()
        {
            return await _context.CartTransactions
                                 .Include(c => c.User)
                                 .Include(c => c.MenuItem)
                                 .ToListAsync();
        }

        public override async Task<CartTransaction> GetById(int key)
        {
            var result = await _context.CartTransactions
                                       .Include(c => c.User)
                                       .Include(c => c.MenuItem)
                                       .FirstOrDefaultAsync(c => c.CartID == key);
            if (result == null)
                throw new NoSuchEntityException();
            return result;
        }
    }
}
