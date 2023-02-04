using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using GusMelfordBot.SimpleKafka.Interfaces;

namespace GusMelfordBot.Api.HostedServices;

public class TryHarderHostedService : IHostedService, IDisposable
{
    private const int ATTEMPT_CONTENT_TEMP = 10;
    
    private Timer? _timer;
    private readonly ILogger<TryHarderHostedService> _logger;
    private readonly IAbyssRepository _abyssRepository;
    private readonly IKafkaProducer<string> _kafkaProducer;

    public TryHarderHostedService(
        ILogger<TryHarderHostedService> logger,
        IAbyssRepository abyssRepository, 
        IKafkaProducer<string> kafkaProducer)
    {
        _logger = logger;
        _abyssRepository = abyssRepository;
        _kafkaProducer = kafkaProducer;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TryHarderHostedService running.");

        _timer = new Timer(TryGetContent, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

        return Task.CompletedTask;
    }

    private async void TryGetContent(object? state)
    {
        foreach (AttemptContent attemptContent in await _abyssRepository.GetAttemptContentAsync(ATTEMPT_CONTENT_TEMP))
        {
            await _kafkaProducer.ProduceAsync(new ContentEvent
            {
                Attempt = attemptContent.Attempt,
                Message = attemptContent.Message,
                SessionId = attemptContent.SessionId,
                GroupId = attemptContent.GroupId,
                UserId = attemptContent.UserId
            });
            
            _logger.LogInformation("Trying to get content: {ContentId}, attempt {Attempt}", 
                attemptContent.SessionId, attemptContent.Attempt);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}