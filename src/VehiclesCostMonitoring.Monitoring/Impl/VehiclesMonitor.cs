using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using VehiclesCostMonitoring.Bot.Services;
using VehiclesCostMonitoring.Bot.Services.Exceptions;
using VehiclesMonitoring.Bot.Data;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehiclesCostMonitoring.Monitoring.Impl
{
    public class VehiclesMonitor : IMonitor
    {
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IProviderFactory _providerFactory;
        private readonly ITelegramBotClient _botClient;

        public VehiclesMonitor(
            IUserRepository userRepository,
            IVehicleRepository vehicleRepository,
            IProviderFactory providerFactory,
            ITelegramBotClient botClient)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
        }
        
        public async Task PerformMonitoringAsync()
        {
            var vehiclesFromDb = await _vehicleRepository.GetAllAsync();

            foreach (var vehicle in vehiclesFromDb)
            {
                // TODO: perform db request to get not archived
                if (vehicle.IsArchived) continue;

                var provider = _providerFactory.GetProvider(vehicle.Url);

                try
                {
                    var newVehicleData = await provider.GetByUrlAsync(vehicle.Url);

                    var newCost = newVehicleData.Costs[0].CostInUsd;
                    var oldCost = vehicle.Costs.Last().CostInUsd;

                    if (newCost != oldCost)
                    {
                        foreach (var vehicleUserId in vehicle.UserIds)
                        {
                            var user = await _userRepository.GetByIdAsync(vehicleUserId);

                            await _botClient.SendTextMessageAsync(user.ChatId,
                                $"Цена на {vehicle.Name} ({vehicle.Url}) изменилась! Новая цена - ${newCost}. Старая цена - ${oldCost}");
                        }
                    }

                    vehicle.Costs = vehicle.Costs.Concat(newVehicleData.Costs).ToArray();
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
                catch (ProviderException exception)
                {
                    switch (exception.Type)
                    {
                        case ProviderExceptionType.ProviderNotFound:
                            break;
                        case ProviderExceptionType.AdClosedOrNotFound:
                            await ArchiveVehicle(vehicle);
                            break;
                        default:
                            throw new NotSupportedException($"Exception type {exception.Type} is not supported");
                    }
                }
            }
        }

        private async Task ArchiveVehicle(Vehicle vehicle)
        {
            vehicle.IsArchived = true;
            await _vehicleRepository.UpdateAsync(vehicle);
            
            foreach (var vehicleUserId in vehicle.UserIds)
            {
                var user = await _userRepository.GetByIdAsync(vehicleUserId);

                await _botClient.SendTextMessageAsync(user.ChatId,
                    $"Объявление {vehicle.Name} ({vehicle.Url}) больше не доступно. Я больше его не отслеживаю.");
            }
        }
    }
}