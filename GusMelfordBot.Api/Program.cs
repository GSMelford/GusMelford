using Confluent.Kafka;
using GusMelfordBot.Api;
using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Api.KafkaEventHandlers.Handlers;
using GusMelfordBot.Api.Settings;
using SimpleKafka.Interfaces;

var builder = WebApplication.CreateBuilder(args);

AppSettings appSettings = new AppSettings();
builder.Configuration.Bind(nameof(AppSettings), appSettings);
builder.Services.ConfigureServices(appSettings);
WebApplication app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapGet("/", () => "GusMelfordBot 2.0");

if (builder.Environment.IsDevelopment())  
{  
    app.UseDeveloperExceptionPage();  
}  
else
{
    app.UseHsts();  
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapHealthChecks("/health"); });
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
SubscribeOnEvents();
app.Run();

void SubscribeOnEvents()
{
    IKafkaConsumerFactory kafkaConsumerFactory = app.Services.GetRequiredService<IKafkaConsumerFactory>();
    kafkaConsumerFactory.Subscribe<TelegramMessageReceivedEvent, TelegramMessageReceivedHandler>(BuildDefaultConsumerConfig());
}

ConsumerConfig BuildDefaultConsumerConfig()
{
    return new ConsumerConfig
    {
        BootstrapServers = appSettings.KafkaSettings.BootstrapServers
    };
}