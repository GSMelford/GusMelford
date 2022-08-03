using GusMelfordBot.Api.KafkaEventHandlers.Events;
using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using GusMelfordBot.SimpleKafka.Interfaces;
namespace GusMelfordBot.Api.HostedServices;

public class ContentCollectorHostedService : IHostedService, IDisposable
{
    private readonly ILogger<ContentCollectorHostedService> _logger;
    private readonly IDatabaseContext _databaseContext;
    private readonly IKafkaProducer<string> _kafkaProducer;

    private Timer _timer = null!;
    private int _executionCount;
    
    public ContentCollectorHostedService(
        ILogger<ContentCollectorHostedService> logger, 
        IDatabaseContext databaseContext, IKafkaProducer<string> kafkaProducer)
    {
        _logger = logger;
        _databaseContext = databaseContext;
        _kafkaProducer = kafkaProducer;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RefreshContent Hosted Service running");
        
        _timer = new Timer(Retry, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
        
        return Task.CompletedTask;
    }

    private void Retry(object? state)
    {
        var count = Interlocked.Increment(ref _executionCount);

        List<Content> contents = _databaseContext.Set<Content>()
            .Where(x => x.IsValid == null || x.IsSaved == false)
            .ToList();

        foreach (Content content in contents)
        {
            if (string.IsNullOrEmpty(content.OriginalLink))
            {
                _databaseContext.Remove(content);
                _databaseContext.SaveChanges();
                continue;
            }

            _ = _kafkaProducer.ProduceAsync(new ContentCollectorMessageEvent
            {
                Id = content.Id,
                MessageText = content.OriginalLink
            }).Result;
        }
        
    }
    
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _timer.Dispose();
    }
}