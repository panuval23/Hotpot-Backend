using HotPot23API.Exceptions;
using HotPot23API.Models;
using System.Collections.Generic;

namespace HotPot23API.Repositories
{
    public class CategoryRepository : Repository<int, CategoryMaster>
    {
        public async override Task<CategoryMaster> GetById(int key)
        {
            var item = list.FirstOrDefault(x => x.CategoryID == key);
            if (item == null)
                throw new NoSuchEntityException();
            return item;
        }
    }
}
