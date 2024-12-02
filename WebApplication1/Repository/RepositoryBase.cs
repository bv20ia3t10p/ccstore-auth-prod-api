using MongoDB.Driver;
using System.Linq.Expressions;
using CcStore.Repository.Contracts;

namespace CcStore.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public RepositoryBase(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T> FindByIdAsync(object id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).ToListAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", typeof(T).GetProperty("Id")?.GetValue(entity, null));
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", typeof(T).GetProperty("Id")?.GetValue(entity, null));
            await _collection.DeleteOneAsync(filter);
        }
    }
}
