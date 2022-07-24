using GusMelfordBot.Api;
using GusMelfordBot.Api.Settings;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = builder.Configuration.BindAppSettings();
builder.Services.ConfigureServices(appSettings);

WebApplication app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
app.InitializeDatabase(appSettings.DatabaseSettings);
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapGet("/", () => "GusMelfordBot 2.0");
app.SetEnvironmentSettings(builder);
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();