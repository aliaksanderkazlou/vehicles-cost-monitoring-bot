using System;
using Microsoft.Extensions.DependencyInjection;

namespace VehiclesCostMonitoring.Bot.Services.Impl
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        
        public IProvider GetProvider(string url)
        {
            return _serviceProvider.GetRequiredService(ProviderTypeStorage.GetByUrl(url)) as IProvider;
        }
    }
}