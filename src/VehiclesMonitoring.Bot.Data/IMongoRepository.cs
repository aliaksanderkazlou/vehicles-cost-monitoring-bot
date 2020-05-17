using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data
{
    public interface IMongoRepository<T> : IRepository<T> where T : BaseModel
    {
        Task<IEnumerable<T>> SearchByFilterAsync(FilterDefinition<T> filter);

        string GenerateNewId();
    }
}