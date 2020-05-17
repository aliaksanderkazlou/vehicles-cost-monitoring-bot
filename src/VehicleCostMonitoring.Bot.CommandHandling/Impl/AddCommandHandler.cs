using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Attributes;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesCostMonitoring.Bot.Services;
using VehiclesCostMonitoring.Bot.Services.Exceptions;
using VehiclesMonitoring.Bot.Data;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    [CommandHandler("/add", new[] { UserStatuses.WaitingForAddVehicleResponse })]
    public class AddCommandHandler : IContextCommandHandler<Message, IEnumerable<HandlerResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IProviderFactory _providerFactory;

        public AddCommandHandler(IUserRepository userRepository, IVehicleRepository vehicleRepository, IProviderFactory providerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
            _providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
        }
        
        public async Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);
            user.Status = UserStatuses.WaitingForAddVehicleResponse;
            await _userRepository.UpdateAsync(user);

            return new[]
            {
                new HandlerResult
                {
                    Message = "Пожалуйта, введите url страницы с автомобилем. Пока что поддерживается только onliner.",
                }
            };
        }

        public async Task<IEnumerable<HandlerResult>> HandleContextAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);

            var existingVehicle = await _vehicleRepository.GetByUrlAsync(message.Text);

            if (existingVehicle != null)
            {
                if (existingVehicle.UserIds.Contains(user.Id))
                {
                    user.Status = null;
                    await _userRepository.UpdateAsync(user);

                    return new[] {new HandlerResult {Message = "Вы уже добавили эту ссылку. /help"}};
                }

                user.VehicleIds = user.VehicleIds.Concat(new[] {existingVehicle.Id}).ToArray();
                await _userRepository.UpdateAsync(user);
                
                existingVehicle.UserIds = existingVehicle.UserIds.Concat(new[] {user.Id}).ToArray();
                await _vehicleRepository.UpdateAsync(existingVehicle);
                    
                // TODO: refactor code duplication
                return new[] {new HandlerResult {Message = "Ссылка сохранена. Я уведомлю вас когда цена изменится. /help"}};
            }

            try
            {
                var vehicle = await _providerFactory.GetProvider(message.Text).GetByUrlAsync(message.Text);

                vehicle.UserIds = new[] {user.Id};
                await _vehicleRepository.InsertAsync(vehicle);

                user.VehicleIds = user.VehicleIds.Concat(new[] {vehicle.Id}).ToArray();
                user.Status = null;
                await _userRepository.UpdateAsync(user);

                return new[] {new HandlerResult {Message = "Ссылка сохранена. Я уведомлю вас когда цена изменится. /help"}};
            }
            catch (ProviderException exception)
            {
                var resultMessage = exception.Type switch
                {
                    ProviderExceptionType.ProviderNotFound =>
                    "Извините, на данный момент мы не поддерживаем данного провайдера, либо вы отправили некорректную ссылку. Попробуйте еще раз.",
                    ProviderExceptionType.AdClosedOrNotFound => "Извините, объявление закрыто или не найдено. Попробуйте еще раз.",
                    _ => throw new NotSupportedException($"Exception with type {exception.Type} is not supported")
                };

                return new[] {new HandlerResult {Message = resultMessage}};
            }
        }
    }
}