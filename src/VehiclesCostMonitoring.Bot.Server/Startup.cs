using System;
using System.Collections.Generic;
using Bot.CommandHandling.Core.Extensions;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;
using VehicleCostMonitoring.Bot.CommandHandling;
using VehicleCostMonitoring.Bot.CommandHandling.Impl;
using VehicleCostMonitoring.Bot.CommandHandling.Models;
using VehiclesCostMonitoring.Bot.Server.Extensions;
using VehiclesCostMonitoring.Bot.Services.Extensions;
using VehiclesCostMonitoring.Common.Settings;
using VehiclesCostMonitoring.Monitoring;
using VehiclesCostMonitoring.Monitoring.Extensions;
using VehiclesMonitoring.Bot.Data.Extensions;

namespace VehiclesCostMonitoring.Bot.Server
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);

            services
                .AddControllers()
                .AddNewtonsoftJson();
            
            services.AddMvc();
            services.AddTelegramBot(Configuration);
            services.AddCommandHandlingDependencies<Message, IEnumerable<HandlerResult>>();
            services.AddDataDependencies();
            services.AddServicesDependencies();
            services.AddMonitoringDependencies();

            services.AddTransient<IMainCommandHandler, CommandHandler>();
            
            var migrationOptions = new MongoMigrationOptions
            {
                Strategy = MongoMigrationStrategy.Migrate,
                BackupStrategy = MongoBackupStrategy.Collections
            };
            
            var storageOptions = new MongoStorageOptions
            {
                CheckConnection = false,
                MigrationOptions = migrationOptions
            };
            
            services.AddHangfire(config =>
            {
                config.UseMongoStorage(Configuration["DatabaseConnectionString"], Configuration["DatabaseName"], storageOptions);
            });
            services.AddHangfireServer();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();
        }
        
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseTelegramBot("/webhook");
            
            app.UseHangfireDashboard();
            
            RecurringJob.AddOrUpdate(() => serviceProvider.GetRequiredService<IMonitor>().PerformMonitoringAsync(), Cron.Daily(15));
        }
    }
}