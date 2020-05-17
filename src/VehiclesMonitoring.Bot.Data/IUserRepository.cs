using System.Threading.Tasks;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesMonitoring.Bot.Data
{
    public interface IUserRepository : IMongoRepository<User>
    {
        Task<User> GetByChatIdAsync(long chatId);
    }
}