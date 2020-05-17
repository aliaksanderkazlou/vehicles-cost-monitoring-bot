using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data.Impl
{
    public class VehicleRepository : MongoRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(MongoConfigurator configurator) : base(configurator) { }

        protected override IMongoCollection<Vehicle> Items => MongoConfigurator.Vehicles;
        public async Task<Vehicle> GetByUrlAsync(string url)
        {
            var filter = Builders<Vehicle>.Filter.Eq(f => f.Url, url);

            var searchResult = await SearchByFilterAsync(filter);

            return searchResult.FirstOrDefault();
        }
    }
}