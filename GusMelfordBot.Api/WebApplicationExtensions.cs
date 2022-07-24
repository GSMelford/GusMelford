using GusMelfordBot.Api.Settings;
using GusMelfordBot.Infrastructure;
using GusMelfordBot.Infrastructure.Interfaces;

namespace GusMelfordBot.Api;

public static class WebApplicationExtensions
{
    public static AppSettings BindAppSettings(this ConfigurationManager configurationManager)
    {
        AppSettings appSettings = new AppSettings();
        configurationManager.Bind(nameof(AppSettings), appSettings);
        return appSettings;
    }

    public static void SetEnvironmentSettings(this WebApplication app, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())  
        {  
            app.UseDeveloperExceptionPage();  
        }
        else
        {
            app.UseHsts();
        }
    }

    public static void InitializeDatabase(this WebApplication app, DatabaseSettings databaseSettings)
    {
        IDatabaseContext databaseContext = app.Services.GetRequiredService<IDatabaseContext>();
        databaseContext.InitializeDatabase(databaseSettings);
    }
}