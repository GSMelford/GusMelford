using Gelf.Extensions.Logging;
using GusMelfordBot.Core.Authentication;
using GusMelfordBot.Core.Domain.Apps;
using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Contents.ContentProviders.Telegram;
using GusMelfordBot.Core.Domain.Apps.ContentDownload.TikTok;
using GusMelfordBot.Core.Domain.Commands;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Domain.Update;
using GusMelfordBot.Core.Extensions;
using GusMelfordBot.Core.Services;
using GusMelfordBot.Core.Services.Apps;
using GusMelfordBot.Core.Services.Apps.ContentCollector;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Contents;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload.TikTok;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Contents.ContentProviders.Telegram;
using GusMelfordBot.Core.Services.Commands;
using GusMelfordBot.Core.Services.GusMelfordBot;
using GusMelfordBot.Core.Services.Modules;
using GusMelfordBot.Core.Services.Requests;
using GusMelfordBot.Core.Services.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

namespace GusMelfordBot.Core;

using Settings;
using Database.Context;
using Database.Interfaces;
using Serilog;
using Serilog.Core;

public static class GusMelfordBotWebApplicationExtensions
{
    public static void AddServices(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddSingleton(appSettings);
        services.AddControllers();
        services.AddHealthChecks();
        services.AddCors();
        
        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
            });
        
        services.AddSpaStaticFiles(options => options.RootPath = "client-app/dist");  
        services.AddHttpClient();
        services.AddTransient<IContentRepository, ContentRepository>();
        services.AddTransient<IContentService, ContentService>();
        services.AddTransient<IContentDownloadRepository, ContentDownloadRepository>();
        services.AddTransient<IContentDownloadService, ContentDownloadService>();
        services.AddTransient<IApplicationRepository, ApplicationRepository>();
        
        services.AddTransient<IGusMelfordBotService, GusMelfordBotService>(
            provider => new GusMelfordBotService(
                appSettings.TelegramBotSettings?.Token, 
                new NullLogger<GusMelfordBotService>(), 
                provider.GetRequiredService<HttpClient>()));
        
        services.AddTransient<ITikTokRepository, TikTokRepository>();
        services.AddTransient<ITikTokService, TikTokService>();
        services.AddTransient<IContentCollectorService, ContentCollectorService>();
        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<IRequestService, RequestService>();
        services.AddTransient<IUpdateService, UpdateService>();
        services.AddTransient<ICommandRepository, CommandRepository>();
        services.AddTransient<ICommandService, CommandService>();
        services.AddTransient<ISystemService, SystemService>();
        services.AddTransient<ISystemRepository, SystemRepository>();
        services.AddTransient<ITikTokDownloaderService, TikTokDownloaderService>();
        services.AddTransient<IFtpServerService, FtpServerService>(
            provider => new FtpServerService(
                appSettings.FtpServerSettings?.FtpUrl ?? string.Empty,
                appSettings.FtpServerSettings?.UserName ?? string.Empty,
                appSettings.FtpServerSettings?.Password ?? string.Empty,
                provider.GetRequiredService<ILogger<FtpServerService>>()));
        
        services.AddHostedService<RefreshContentService>();
        services.AddHostedService<LongPoolingUpdateService>();
        services.AddTransient<IDatabaseManager>(
            provider => new DatabaseManager(appSettings.DatabaseSettings, 
                provider.GetRequiredService<ILogger<ApplicationContext>>()));
        services.AddTransient<ITelegramService, TelegramService>();
        services.AddTransient<IDataLakeService, DataLakeService>();
    }
    
    public static Logger AddGraylog(this WebApplicationBuilder builder, AppSettings appSettings)
    {  
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        
        return logger;
    }
}