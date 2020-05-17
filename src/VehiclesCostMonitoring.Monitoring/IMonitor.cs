using System.Threading.Tasks;

namespace VehiclesCostMonitoring.Monitoring
{
    public interface IMonitor
    {
        Task PerformMonitoringAsync();
    }
}