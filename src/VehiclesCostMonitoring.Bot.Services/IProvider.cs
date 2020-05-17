using System.Threading.Tasks;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesCostMonitoring.Bot.Services
{
    public interface IProvider
    {
        Task<Vehicle> GetByUrlAsync(string url);
    }
}