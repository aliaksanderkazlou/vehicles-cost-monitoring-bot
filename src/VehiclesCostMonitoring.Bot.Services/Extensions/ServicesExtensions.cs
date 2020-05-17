using Microsoft.Extensions.DependencyInjection;
using VehiclesCostMonitoring.Bot.Services.Impl;

namespace VehiclesCostMonitoring.Bot.Services.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection collection)
        {
            ProviderTypeStorage.Initialize();

            var types = ProviderTypeStorage.GetAll();

            foreach (var type in types)
            {
                collection.AddTransient(type);
            }
            
            collection.AddTransient<IProviderFactory, ProviderFactory>();

            return collection;
        }
    }
}