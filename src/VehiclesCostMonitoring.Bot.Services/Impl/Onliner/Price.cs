using System.Collections.Generic;

namespace VehiclesCostMonitoring.Bot.Services.Impl.Onliner
{
    public class Price
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
        public Dictionary<string, CurrencyItem> Converted { get; set; }
    }
}