using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Exceptions;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesMonitoring.Bot.Data;
using VehiclesMonitoring.Bot.Data.Models;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    public class CommandHandler : IMainCommandHandler
    {
        private readonly ICommandHandlerFactory<Message, IEnumerable<HandlerResult>> _factory;
        private readonly IUserRepository _userRepository;

        public CommandHandler(ICommandHandlerFactory<Message, IEnumerable<HandlerResult>> factory, IUserRepository userRepository)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        public async Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var user = await _userRepository.GetByChatIdAsync(message.UserInfo.ChatId);

            if (user is null)
            {
                return await HandleMainCommandAsync(message);
            }

            var action = user.Status is null || message.Text.StartsWith('/')
                ? HandleMainCommandAsync(message)
                : HandleUserPendingActionAsync(message, user.Status);
            
            try
            {
                return await action;
            }
            catch (CommandHandlingException exception)
            {
                switch (exception.Type)
                {
                    case CommandHandlingExceptionType.HandlerNotFound:
                        return new[] { new HandlerResult { Message = "Извините, я не поддерживаю такую команду..." } };
                    default:
                        throw new NotSupportedException($"Exception type {exception.Type} is not supported");
                }
            }
        }

        private async Task<IEnumerable<HandlerResult>> HandleMainCommandAsync(Message message)
        {
            return await _factory.GetByCommand(message.Text.Split(" ")[0]).HandleAsync(message);
        }
        
        private async Task<IEnumerable<HandlerResult>> HandleUserPendingActionAsync(Message message, string status)
        {
            return await _factory.GetByContextStatus(status).HandleContextAsync(message);
        }
    }
}