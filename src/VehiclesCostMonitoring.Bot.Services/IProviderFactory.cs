namespace VehiclesCostMonitoring.Bot.Services
{
    public interface IProviderFactory
    {
        IProvider GetProvider(string url);
    }
}