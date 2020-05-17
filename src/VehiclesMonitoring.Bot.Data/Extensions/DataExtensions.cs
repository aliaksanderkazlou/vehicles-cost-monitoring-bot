using Microsoft.Extensions.DependencyInjection;
using VehiclesMonitoring.Bot.Data.Impl;

namespace VehiclesMonitoring.Bot.Data.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataDependencies(this IServiceCollection collection)
        {
            collection.AddTransient<MongoConfigurator>();
            collection.AddTransient<IUserRepository, UserRepository>();
            collection.AddTransient<IVehicleRepository, VehicleRepository>();
            
            return collection;
        }
    }
}