using GusMelfordBot.Api.Settings;
using GusMelfordBot.Domain.Application.ContentCollector;
using GusMelfordBot.Events;
using Kyoto.Kafka.Interfaces;

namespace GusMelfordBot.Api.HostedServices;

public class TryHarderHostedService : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly AbyssSettings _abyssSettings;
    private readonly ILogger<TryHarderHostedService> _logger;
    private readonly IAbyssRepository _abyssRepository;
    private readonly IKafkaProducer<string> _kafkaProducer;

    public TryHarderHostedService(
        AppSettings appSettings,
        ILogger<TryHarderHostedService> logger,
        IAbyssRepository abyssRepository, 
        IKafkaProducer<string> kafkaProducer)
    {
        _logger = logger;
        _abyssRepository = abyssRepository;
        _kafkaProducer = kafkaProducer;
        _abyssSettings = appSettings.FeatureSettings.AbyssSettings;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TryHarderHostedService running.");

        _timer = new Timer(TryGetContentAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(_abyssSettings.MinuteTimeBetweenAttempts));

        return Task.CompletedTask;
    }

    private async void TryGetContentAsync(object? state)
    {
        var contents = 
            await _abyssRepository.GetAttemptContentAsync(_abyssSettings.NumberOfAttempt);
        
        foreach (AttemptContent attemptContent in contents)
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