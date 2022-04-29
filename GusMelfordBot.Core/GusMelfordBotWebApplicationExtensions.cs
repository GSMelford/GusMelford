using GusMelfordBot.Core.Authentication;
using GusMelfordBot.Core.Domain.Apps;
using GusMelfordBot.Core.Domain.Apps.ContentCollector;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Domain.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Domain.Commands;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Domain.System;
using GusMelfordBot.Core.Domain.Telegram;
using GusMelfordBot.Core.Domain.Update;
using GusMelfordBot.Core.Services;
using GusMelfordBot.Core.Services.Apps;
using GusMelfordBot.Core.Services.Apps.ContentCollector;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Content;
using GusMelfordBot.Core.Services.Apps.ContentCollector.Content.ContentProviders.TikTok;
using GusMelfordBot.Core.Services.Apps.ContentCollector.ContentDownload;
using GusMelfordBot.Core.Services.Commands;
using GusMelfordBot.Core.Services.GusMelfordBot;
using GusMelfordBot.Core.Services.Requests;
using GusMelfordBot.Core.Services.System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace GusMelfordBot.Core;

using Extensions;
using Settings;
using Database.Context;
using Database.Interfaces;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;

public static class GusMelfordBotWebApplicationExtensions
{
    public static void AddServices(this IServiceCollection services, CommonSettings commonSettings)
    {
        services.AddSingleton(commonSettings);
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
                commonSettings.TelegramBotSettings?.Token, 
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
        services.AddTransient<IFtpServerService, FtpServerService>(
            _ => new FtpServerService(
                commonSettings.FtpServerSettings.FtpUrl,
                commonSettings.FtpServerSettings.UserName,
                commonSettings.FtpServerSettings.Password));
        
        services.AddTransient<IDatabaseManager>(
            _ => new DatabaseManager(commonSettings.DatabaseSettings));
    }
    
    public static Logger AddGraylog(
        this WebApplicationBuilder builder,
        CommonSettings commonSettings)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.Graylog(new GraylogSinkOptions
            {
                Host = commonSettings.GrayLogSettings?.Host,
                Port = commonSettings.GrayLogSettings?.Port.ToInt(),
                TransportType = commonSettings.GrayLogSettings?.TransportType switch
                {
                    "Udp" => TransportType.Udp,
                    "Tcp" => TransportType.Tcp,
                    "Http" => TransportType.Http,
                    _ => throw new ArgumentNullException()
                }
            })
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        return logger;
    }
}