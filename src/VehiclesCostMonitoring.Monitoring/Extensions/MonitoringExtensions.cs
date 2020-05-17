using Microsoft.Extensions.DependencyInjection;
using VehiclesCostMonitoring.Monitoring.Impl;

namespace VehiclesCostMonitoring.Monitoring.Extensions
{
    public static class MonitoringExtensions
    {
        public static IServiceCollection AddMonitoringDependencies(this IServiceCollection collection)
        {
            collection.AddTransient<IMonitor, VehiclesMonitor>();

            return collection;
        }
    }
}