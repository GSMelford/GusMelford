using GusMelfordBot.Api;
using GusMelfordBot.Api.Settings;
using GusMelfordBot.Api.WebSoketHandlers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = builder.Configuration.BindAppSettings();
builder.Services.ConfigureServices(appSettings);
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
    x =>
    {
        x.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    }));

WebApplication app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.InitializeDatabase(appSettings.DatabaseSettings);
app.UseCors("CorsPolicy");
app.MapGet("/", () => "GusMelfordBot 2.0");
app.SetEnvironmentSettings(builder);
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.SubscribeOnEvents(appSettings);
app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.UseEndpoints(endpoints => { endpoints.MapHub<ContentCollectorHub>("/content-viewer-hub"); });
await app.RunAsync();
