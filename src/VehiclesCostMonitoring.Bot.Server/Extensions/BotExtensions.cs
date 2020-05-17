using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using VehiclesCostMonitoring.Common.Settings;

namespace VehiclesCostMonitoring.Bot.Server.Extensions
{
    public static class BotExtensions
    {
        public static void AddTelegramBot(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var settings = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>();
            var client = new TelegramBotClient(settings.Value.BotToken);
            services.AddSingleton<ITelegramBotClient>(client);
        }

        public static IApplicationBuilder UseTelegramBot(this IApplicationBuilder app, string webhookPath)
        {
            var settings = app.ApplicationServices.GetService<IOptions<AppSettings>>();
            var botClient = app.ApplicationServices.GetService<ITelegramBotClient>();

            botClient.SetWebhookAsync(settings.Value.Domain.TrimEnd('/') + webhookPath)
                .GetAwaiter()
                .GetResult();

            return app;
        }
    }
}