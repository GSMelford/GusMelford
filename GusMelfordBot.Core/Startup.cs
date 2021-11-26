namespace GusMelfordBot.Core
{
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
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("<!DOCTYPE html>\n" +
                                                      "<html lang=\"en\">\n" +
                                                      "<head>\n    " +
                                                      "<meta charset=\"UTF-8\">\n    " +
                                                      "<meta http-equiv=\"X-UA-Compatible\" " +
                                                      "content=\"IE=edge\">\n    " +
                                                      "<meta name=\"viewport\"" +
                                                      " content=\"width=device-width, initial-scale=1.0\">\n\n   " +
                                                      " <style>\n        .body-container{\n            " +
                                                      "background: black;\n            " +
                                                      "text-align: center;\n           " +
                                                      " margin: 40;\n            padding: 40;\n    " +
                                                      "    }\n\n        .version{\n         " +
                                                      "   font-weight: bolder;\n         " +
                                                      "   color: grey;\n        }\n    </style>\n\n " +
                                                      "   <title>GusMelfordBot</title>\n</head>\n<body class=\"body-container\">\n " +
                                                      "   <h1 class=\"version\">" +
                                                      $"GusMelfordBot v{commonSettings.Version}" +
                                                      "</h1>\n</body>\n</html>");
                });
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
            logger.LogInformation("TBot started. Time: {Time}", DateTime.UtcNow);
        }
    }
}