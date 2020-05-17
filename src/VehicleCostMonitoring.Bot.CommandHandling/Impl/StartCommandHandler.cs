using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Attributes;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesMonitoring.Bot.Data;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    [CommandHandler("/start")]
    public class StartCommandHandler : ICommandHandler<Message, IEnumerable<HandlerResult>>
    {
        private readonly IUserRepository _userRepository;
        
        private const string HelpText = "/add - Начать мониторинг объявления\n" +
                                        "/remove - Остановить мониторинг объявления\n" +
                                        "/cancel - Отменить текущее действие\n" +
                                        "/help - Узнать что я умею";
        
        public StartCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var existingUser = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);

            if (existingUser is null)
            {
                var user = new User
                {
                    ChatId = message.UserInfo.ChatId,
                    FirstName = message.UserInfo.FirstName,
                    LastName = message.UserInfo.LastName,
                    VehicleIds = new string[0],
                };

                await _userRepository.InsertAsync(user);
            }

            return new[]
            {
                new HandlerResult
                {
                    Message = "Привет, я - VehiclesCostMonitor бот. Я слежу за ценами на " +
                              "указанные автомобили и уведомляю, когда что-то меняется. " +
                              "Пожалуйста, посмотрите, что я умею:",
                },
                new HandlerResult
                {
                    Message = HelpText,
                }
            };
        }
    }
}