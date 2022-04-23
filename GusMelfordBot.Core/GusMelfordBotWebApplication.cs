using GusMelfordBot.Core.Middlewares;
using GusMelfordBot.Core.Settings;
using ILogger = Serilog.ILogger;

namespace GusMelfordBot.Core;

public class GusMelfordBotWebApplication
{
    public void Start(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        
        CommonSettings commonSettings = new CommonSettings();
        builder.Configuration.Bind(nameof(CommonSettings), commonSettings);
        builder.Services.AddServices(commonSettings);
        ILogger logger = builder.AddGraylog(commonSettings);
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        WebApplication app = builder.Build();
        
        if (builder.Environment.IsDevelopment())  
        {  
            app.UseDeveloperExceptionPage();  
        }  
        else  
        {
            app.UseHsts();  
        }  
        
        app.UseRouting();
        app.UseStaticFiles();  
        app.UseMiddleware(typeof(ExceptionMiddleware));
        app.UseDeveloperExceptionPage();
        app.UseStatusCodePages();
        app.UseCors(x => 
            x.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGet("/", SetStartPage);
        });

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        logger.Information("GusMelfordBot WebApplication launched. Time: {Time}", DateTime.UtcNow);
        
        app.Run();
    }
    
    private static Task SetStartPage(HttpContext context)
    {
        context.Response.Redirect(context.Request.Scheme + "://" + context.Request.Host + "/home.html");
        return Task.CompletedTask;
    }
}