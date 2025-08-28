using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Models;
using Microsoft.EntityFrameworkCore;

namespace HotPot23API.Repositories
{
    public class CategoryRepositoryDB : RepositoryDB<int, CategoryMaster>
    {
        public CategoryRepositoryDB(FoodDeliveryManagementContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<CategoryMaster>> GetAll()
        {
            return await _context.CategoryMasters.ToListAsync();
        }

        public override async Task<CategoryMaster> GetById(int key)
        {
            var result = await _context.CategoryMasters
                .FirstOrDefaultAsync(c => c.CategoryID == key);
            if (result == null)
                throw new NoSuchEntityException();
            return result;
        }
    }
}
