using GusMelfordBot.Core.Interfaces;
using GusMelfordBot.Core.Services;

namespace GusMelfordBot.Core
{
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
            
            services.AddSingleton<IGusMelfordBotService>(
                provider => new GusMelfordBotServiceService(provider.GetRequiredService<CommonSettings>().TelegramBotSettings));
            services.AddSingleton<IDatabaseContext>(
                provider => new DatabaseManager(provider.GetRequiredService<CommonSettings>().DatabaseSettings));
            services.AddSingleton<ITikTokService>(
                provider => new TikTokService(
                    provider.GetRequiredService<IDatabaseContext>(), 
                    provider.GetRequiredService<IGusMelfordBotService>()));
        }
    }
}