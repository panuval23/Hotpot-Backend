using HotPot23API.Exceptions;
using HotPot23API.Interfaces;

namespace HotPot23API.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T : class
    {
        protected static List<T> list = new List<T>();

        public async Task<T> Add(T entity)
        {
            list.Add(entity);
            return entity;
        }

        public async Task<T> Delete(K key)
        {
            var item = await GetById(key);
            if (item != null)
            {
                list.Remove(item);
                return item;
            }
            throw new NoSuchEntityException();
        }

        public async Task<IEnumerable<T>> GetAll()
        {

            return list;
        }

        public abstract Task<T> GetById(K key);


        public async Task<T> Update(K key, T entity)
        {
            var item = await GetById(key);
            if (item != null)
            {
                list.Remove(item);
                list.Add(entity);
                return item;
            }
            throw new NoSuchEntityException();
        }
    }
}
