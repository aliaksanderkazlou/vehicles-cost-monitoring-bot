using System;

namespace VehiclesCostMonitoring.Bot.Services.Attributes
{
    public class ProviderAttribute : Attribute
    {
        public string Url { get; }

        public ProviderAttribute(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}