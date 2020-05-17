using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VehiclesCostMonitoring.Common.Settings;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data
{
    public class MongoConfigurator
    {
        private readonly IMongoDatabase _database;

        public MongoConfigurator(IOptions<AppSettings> options)
        {
            var settings = options.Value;
            
            var client = new MongoClient(settings.DatabaseConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");

        public IMongoCollection<Vehicle> Vehicles => _database.GetCollection<Vehicle>("vehicles");
    }
}