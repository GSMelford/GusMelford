namespace GusMelfordBot.Core;

using Applications;
using Applications.Commander;
using Applications.MemesChatApp;
using Applications.MemesChatApp.ContentProviders.TikTok;
using GusMelfordBot.Core.Applications.MemesChatApp.Interfaces;
using Applications.MemesChatApp.Player;
using Interfaces;
using GusMelfordBot.Core.Services.GusMelfordBot;
using Services.Requests;
using GusMelfordBot.Core.Services.System;
using Services.Update;
using Settings;
using Database.Context;
using GusMelfordBot.Database.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

public static class GusMelfordBotWebApplicationExtensions
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
    
    public static Serilog.ILogger AddGraylog(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        
        return logger;
    }
}