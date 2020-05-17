using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Attributes;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesMonitoring.Bot.Data;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    [CommandHandler("/remove", new []{ UserStatuses.WaitingForRemoveVehicleResponse })]
    public class RemoveCommandHandler : IContextCommandHandler<Message, IEnumerable<HandlerResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository;

        public RemoveCommandHandler(IUserRepository userRepository, IVehicleRepository vehicleRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
        }

        public async Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);

            var vehicles = new List<Vehicle>();

            foreach (var id in user.VehicleIds)
            {
                vehicles.Add(await _vehicleRepository.GetByIdAsync(id));
            }

            if (!vehicles.Any())
            {
                return new[] {new HandlerResult {Message = "Извините, я еще не слежу ни за одним объявлением. /help"}};
            }

            user.Status = UserStatuses.WaitingForRemoveVehicleResponse;
            await _userRepository.UpdateAsync(user);

            return new[]
            {
                new HandlerResult
                {
                    Message = "Пожалуйста, выберите объявление:",
                    NeedKeyBoard = true,
                    KeyBoardOptions = vehicles.Select(vehicle => $"{vehicle.Name} - ${vehicle.Costs.Last().CostInUsd}").ToArray()
                },
            };
        }

        public async Task<IEnumerable<HandlerResult>> HandleContextAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);

            var vehicles = new List<Vehicle>();

            foreach (var id in user.VehicleIds)
            {
                vehicles.Add(await _vehicleRepository.GetByIdAsync(id));
            }

            var vehicle = vehicles.SingleOrDefault(v => message.Text.Equals($"{v.Name} - ${v.Costs.Last().CostInUsd}"));

            if (vehicle is null)
            {
                return new[]
                {
                    new HandlerResult
                    {
                        Message = "Я не нашел такого среди списка ваших объявлений. Пожалуйста, выберите объявление:",
                        NeedKeyBoard = true,
                        KeyBoardOptions = vehicles.Select(v => $"{v.Name} - ${v.Costs.Last().CostInUsd}").ToArray()
                    },
                };
            }

            vehicle.UserIds = vehicle.UserIds.Except(new[] {user.Id}).ToArray();
            if (!vehicle.UserIds.Any())
            {
                await _vehicleRepository.DeleteAsync(vehicle.Id);
            }
            else
            {
                await _vehicleRepository.UpdateAsync(vehicle);
            }

            user.VehicleIds = user.VehicleIds.Except(new[] {vehicle.Id}).ToArray();
            user.Status = null;
            await _userRepository.UpdateAsync(user);

            return new[] {new HandlerResult {Message = "Объявление успешно удалено. /help"}};
        }
    }
}