using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleCostMonitoring.Bot.CommandHandling.Models;

namespace VehicleCostMonitoring.Bot.CommandHandling
{
    public interface IMainCommandHandler
    {
        Task<IEnumerable<HandlerResult>> HandleAsync(Message message);
    }
}