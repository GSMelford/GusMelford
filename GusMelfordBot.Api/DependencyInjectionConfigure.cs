using System.Text;
using Confluent.Kafka;
using GusMelfordBot.Api.HostedServices;
using GusMelfordBot.Api.Services.Applications.ContentCollector;
using GusMelfordBot.Api.Services.Auth;
using GusMelfordBot.Api.Services.Telegram;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Application;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Domain.Auth;
using GusMelfordBot.Domain.Telegram;
using GusMelfordBot.Extensions.Services.DataLake;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Repositories.Application;
using GusMelfordBot.Infrastructure.Repositories.Application.ContentCollector;
using GusMelfordBot.Infrastructure.Repositories.Auth;
using GusMelfordBot.Infrastructure.Repositories.Telegram;
using GusMelfordBot.SimpleKafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TBot.Client;

namespace GusMelfordBot.Api;

public static class DependencyInjectionConfigure
{
    public static void ConfigureServices(this IServiceCollection serviceCollection, AppSettings appSettings)
    {
        serviceCollection.AddHttpClient();
        serviceCollection.AddSignalR();
        serviceCollection.AddTBotClient(appSettings.TelegramBotSettings!.Token);
        serviceCollection.AddTransient<IDatabaseContext, DatabaseContext>(_ => new DatabaseContext(appSettings.DatabaseSettings));
        serviceCollection.AddTransient<IUpdateService, UpdateService>();
        serviceCollection.AddSingleton<IContentCollectorRoomFactory, ContentCollectorRoomFactory>();
        serviceCollection.AddTransient<IAuthService, AuthService>();
        serviceCollection.AddTransient<IDataLakeService, DataLakeService>();
        serviceCollection.AddTransient<IAuthRepository, AuthRepository>();
        serviceCollection.AddTransient<IApplicationRepository, ApplicationRepository>();
        serviceCollection.AddTransient<IContentCollectorRepository, ContentCollectorRepository>();
        serviceCollection.AddTransient<IContentCollectorService, ContentCollectorService>();
        serviceCollection.AddTransient<ICommandService, CommandService>();
        serviceCollection.AddTransient<ICommandRepository, CommandRepository>();
        serviceCollection.AddSingleton<ILongCommandService, LongCommandService>();
        serviceCollection.AddSingleton(appSettings);
        serviceCollection.AddHealthChecks();
        serviceCollection.AddControllers();
        serviceCollection.AddHostedService<ContentCollectorHostedService>();
        serviceCollection.AddKafkaProducer<string>(new ProducerConfig
        {
            BootstrapServers = appSettings.KafkaSettings!.BootstrapServers
        });
        serviceCollection.AddKafkaConsumersFactory();
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = appSettings.AuthSettings?.Issuer,
                    ValidateAudience = true,
                    ValidAudience = appSettings.AuthSettings?.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.AuthSettings!.Key)),
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/content-viewer-hub")) {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }
}