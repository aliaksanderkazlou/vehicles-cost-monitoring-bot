using System.Threading.Tasks;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data
{
    public interface IVehicleRepository : IMongoRepository<Vehicle>
    {
        Task<Vehicle> GetByUrlAsync(string url);
    }
}