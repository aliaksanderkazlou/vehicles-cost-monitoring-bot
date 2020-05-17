using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Attributes;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesMonitoring.Bot.Data;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    [CommandHandler("/cancel")]
    public class CancelCommandHandler : ICommandHandler<Message, IEnumerable<HandlerResult>>
    {
        private readonly IUserRepository _userRepository;

        public CancelCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        public async Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);
        
            if (user.Status != null)
            {
                user.Status = null;
                await _userRepository.UpdateAsync(user);
        
                return new[] {new HandlerResult {Message = "Команда отменена. /help"}};
            }
        
            return new[] {new HandlerResult {Message = "Мне нечего отменить. /help"}};
        }
    }
}