namespace GusMelfordBot.Core;

using System.Threading.Tasks;
using Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddServices(Configuration);
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        ILogger<Startup> logger,
        CommonSettings commonSettings)
    {
        if (env.IsDevelopment())
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
        logger.LogInformation("GusMelfordBot started. Time: {Time}", DateTime.UtcNow);
    }

    private static Task SetStartPage(HttpContext context)
    {
        context.Response.Redirect(context.Request.Scheme + "://" + context.Request.Host + "/home.html");
        return Task.CompletedTask;
    }
}