using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VehicleCostMonitoring.Bot.CommandHandling;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using Message = VehicleCostMonitoring.Bot.CommandHandling.Models.Message;

namespace VehiclesCostMonitoring.Bot.Server.Controllers
{
    [Route("/webhook")]
    public class BotController : Controller
    {
        private readonly ITelegramBotClient _client;

        private readonly IMainCommandHandler _handler;

        public BotController(ITelegramBotClient client, IMainCommandHandler handler)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            if (update?.Message is null || update.Type != UpdateType.Message || update.Message.Type != MessageType.Text)
            {
                return Ok();
            }

            var message = new Message
            {
                Text = update.Message.Text,
                UserInfo = new UserInfo
                {
                    FirstName = update.Message.From.FirstName,
                    LastName = update.Message.From.LastName,
                    ChatId = update.Message.Chat.Id
                },
                ChatInfo = new ChatInfo
                {
                    Type = update.Message.Chat.Type.ToString(),
                    Id = update.Message.Chat.Id,
                    UserName = update.Message.Chat.Username
                }
            };

            foreach (var result in await _handler.HandleAsync(message))
            {
                if (result.NeedKeyBoard)
                {
                    await _client.SendTextMessageAsync(message.UserInfo.ChatId,
                        result.Message,
                        replyMarkup: BuildKeyboardMarkup(result.KeyBoardOptions.ToList()));
                }
                else
                {
                    await _client.SendTextMessageAsync(message.UserInfo.ChatId, result.Message, replyMarkup: new ReplyKeyboardRemove());
                }
            }

            return Ok();
        }

        private ReplyKeyboardMarkup BuildKeyboardMarkup(List<string> options)
        {
            var list = new List<KeyboardButton[]>();
            for (var index = 0; index < options.Count; index += 2)
            {
                if (index + 1 <= options.Count - 1)
                {
                    list.Add(new[]
                    {
                        new KeyboardButton(options[index]), 
                        new KeyboardButton(options[index + 1])
                    });
                }
                else
                {
                    list.Add(new[] { new KeyboardButton(options[index]) });
                }
            }
            var keyboard = list.ToArray();

            var replyKeyBoard = new ReplyKeyboardMarkup(keyboard);

            return replyKeyBoard;
        }
    }
}