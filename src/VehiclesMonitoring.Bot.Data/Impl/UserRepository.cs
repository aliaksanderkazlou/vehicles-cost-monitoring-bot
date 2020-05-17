using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data.Impl
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        protected override IMongoCollection<User> Items => MongoConfigurator.Users;
        
        public UserRepository(MongoConfigurator configurator) : base(configurator) { }
        
        public async Task<User> GetByChatIdAsync(long chatId)
        {
            var filter = Builders<User>.Filter.Eq(f => f.ChatId, chatId);

            var searchResult = await SearchByFilterAsync(filter);

            return searchResult.FirstOrDefault();
        }
    }
}