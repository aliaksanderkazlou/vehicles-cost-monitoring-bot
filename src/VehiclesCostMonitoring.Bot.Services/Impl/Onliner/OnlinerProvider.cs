using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using VehiclesCostMonitoring.Bot.Services.Attributes;
using VehiclesCostMonitoring.Bot.Services.Exceptions;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesCostMonitoring.Bot.Services.Impl.Onliner
{
    [Provider("https://ab.onliner.by/")]
    public class OnlinerProviderService : IProvider
    {
        public async Task<Vehicle> GetByUrlAsync(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var urlParts = url.Split('/');
            var realUrl = $"https://ab.onliner.by/sdapi/ab.api/vehicles/{urlParts[^1]}";

            var response = await httpClient.GetAsync(realUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new ProviderException(ProviderExceptionType.AdClosedOrNotFound);
            }

            var onlinerResponse = await response.Content.ReadAsAsync<OnlinerResponse>();

            var usdCurrencyIdentifier = "USD";
            var cost = onlinerResponse.Price.Converted[usdCurrencyIdentifier];

            return new Vehicle
            {
                Name = onlinerResponse.Title,
                IdFromPlatform = onlinerResponse.Id.ToString(),
                IsArchived = false,
                Url = url,
                UserIds = new string[0],
                Costs = new[] {new Cost {CostInUsd = decimal.Parse(cost.Amount), DateTime = DateTime.Now}}
            };
        }
    }
}