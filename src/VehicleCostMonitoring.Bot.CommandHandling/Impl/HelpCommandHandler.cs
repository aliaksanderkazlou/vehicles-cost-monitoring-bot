using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.CommandHandling.Core;
using Bot.CommandHandling.Core.Attributes;
using VehicleCostMonitoring.Bot.CommandHandling.Models;

namespace VehicleCostMonitoring.Bot.CommandHandling.Impl
{
    [CommandHandler("/help")]
    public class HelpCommandHandler : ICommandHandler<Message, IEnumerable<HandlerResult>>
    {
        public Task<IEnumerable<HandlerResult>> HandleAsync(Message message)
        {
            var text = "/add - Начать мониторинг объявления\n" +
                       "/remove - Остановить мониторинг объявления\n" +
                       "/cancel - Отменить текущее действие\n" +
                       "/help - Узнать что я умею";

            var result = new[] { new HandlerResult {Message = text} };

            return Task.FromResult(result.AsEnumerable());
        }
    }
}