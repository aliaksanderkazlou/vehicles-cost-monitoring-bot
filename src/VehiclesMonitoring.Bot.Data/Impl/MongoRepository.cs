using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data.Impl
{
    public abstract class MongoRepository<T> : IMongoRepository<T> where T : BaseModel
    {
        protected abstract IMongoCollection<T> Items { get; }

        protected readonly MongoConfigurator MongoConfigurator;

        protected MongoRepository(MongoConfigurator configurator)
        {
            MongoConfigurator = configurator;
        }
        
        public async Task InsertAsync(T item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                item.Id = GenerateNewId();
            }

            await Items.InsertOneAsync(item);
        }

        public async Task UpdateAsync(T item)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, item.Id);

            await Items.ReplaceOneAsync(filter, item);
        }

        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, id);

            await Items.DeleteOneAsync(filter);
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(f => f.Id, id);

            var searchResult = await Items.FindAsync(filter);

            return searchResult.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var searchResult = await Items.FindAsync(_ => true);
            
            return await searchResult.ToListAsync();
        }

        public async Task<IEnumerable<T>> SearchByFilterAsync(FilterDefinition<T> filter)
        {
            var searchResult = await Items.FindAsync(filter);
                
            return await searchResult.ToListAsync();
        }

        public string GenerateNewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}