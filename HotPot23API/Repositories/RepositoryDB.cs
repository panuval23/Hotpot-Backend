using HotPot23API.Contexts;
using HotPot23API.Interfaces;

namespace HotPot23API.Repositories
{
    public abstract class RepositoryDB<K, T> : IRepository<K, T> where T : class
    {
        protected readonly FoodDeliveryManagementContext _context;

        public RepositoryDB(FoodDeliveryManagementContext context)
        {
            _context = context;
        }
        public async Task<T> Add(T entity)
        {
            _context.ChangeTracker.Clear();
            _context.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Delete(K key)
        {
            _context.ChangeTracker.Clear();
            var obj = await GetById(key);
            _context.Remove(obj);
            await _context.SaveChangesAsync();
            return obj;
        }

        public abstract Task<IEnumerable<T>> GetAll();

        public abstract Task<T> GetById(K key);


        public async Task<T> Update(K key, T entity)
        {
            _context.ChangeTracker.Clear();
            var obj = await GetById(key);
            _context.Entry<T>(obj).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
