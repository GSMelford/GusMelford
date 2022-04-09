using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace GusMelfordBot.Core;

public class GusMelfordBotWebApplication
{
    public void Start(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        ILogger logger = builder.AddGraylog();
        builder.Services.AddServices(builder.Configuration);
        
        WebApplication app = builder.Build();
        if (builder.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseStaticFiles();

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