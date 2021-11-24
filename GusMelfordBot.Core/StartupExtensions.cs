namespace GusMelfordBot.Core
{
    using Interfaces;
    using Services;
    using Services.Data;
    using Database.Context;
    using Settings;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Database.Interfaces;
    
    public static class StartupExtensions
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            CommonSettings commonSettings = new CommonSettings();
            configuration.Bind(nameof(CommonSettings), commonSettings);
            
            services.AddSingleton(commonSettings);
            services.AddControllers();
            services.AddHealthChecks();
            services.AddTransient<IDataService, DataService>();
            
            services.AddSingleton<IGusMelfordBotService>(
                provider => new GusMelfordBotService(provider.GetRequiredService<CommonSettings>().TelegramBotSettings));
            services.AddSingleton<IDatabaseManager>(
                provider => new DatabaseManager(provider.GetRequiredService<CommonSettings>().DatabaseSettings));
            services.AddSingleton<ITikTokService>(
                provider => new TikTokService(
                    provider.GetRequiredService<IDatabaseManager>(), 
                    provider.GetRequiredService<IGusMelfordBotService>()));
        }
    }
}