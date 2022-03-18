using GusMelfordBot.Core.Applications.Commander;
using GusMelfordBot.Core.Applications.MemesChatApp.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.GusMelfordBot;
using GusMelfordBot.Core.Services.System;
using GusMelfordBot.Core.Services.Update;
using GusMelfordBot.Database.Context;
using GusMelfordBot.Database.Interfaces;

namespace GusMelfordBot.Core
{
    using Interfaces;
    using Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Applications.MemesChatApp.Player;
    using Services;
    using Services.Requests;
    using Applications;
    using Applications.MemesChatApp;
    using GusMelfordBot.Core.Applications.MemesChatApp.Interfaces;
    
    public static class StartupExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            CommonSettings commonSettings = new CommonSettings();
            configuration.Bind(nameof(CommonSettings), commonSettings);
            
            services.AddSingleton(commonSettings);
            services.AddControllers();
            services.AddHealthChecks();

            services.AddTransient<IDatabaseManager>(
                provider => new DatabaseManager(provider.GetRequiredService<CommonSettings>().DatabaseSettings));

            services.AddTransient<ICommanderService, CommanderService>();
            services.AddTransient<IUpdateService, UpdateService>();
            services.AddTransient<ISystemService, SystemService>();
            services.AddTransient<IGusMelfordBotService, GusMelfordBotService>();
            services.AddSingleton<IRequestService, RequestService>();
            services.AddTransient<ITikTokService, TikTokService>();
            services.AddSingleton<IPlayerService, PlayerService>();
            services.AddTransient<IMemeChatService, MemeChatService>();
            services.AddTransient<IApplicationService, ApplicationService>();
        }
    }
}