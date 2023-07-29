using Confluent.Kafka;
using Kyoto.Kafka.Interfaces;
using Kyoto.Kafka.Modules;
using Microsoft.Extensions.Logging;

namespace Kyoto.Kafka.Services;

public class KafkaConsumerService : IKafkaConsumerService
{
    private readonly ILogger<IKafkaConsumerService>? _logger;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly ConsumerConfig _config;
    private IConsumer<Ignore, string>? _consumer;
    
    public event EventHandler<ReceivedEventDetails>? Received;

    public KafkaConsumerService(ConsumerConfig? config, ILogger<IKafkaConsumerService>? logger = null)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _config = config ?? new ConsumerConfig();
    }

    public void Consume(string topic, string groupId, bool enableAutoCommit)
    {
        _config.GroupId = groupId;
        _config.EnableAutoCommit = enableAutoCommit;
        _consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
        _consumer.Subscribe(topic);
        
        Task.Factory.StartNew(() =>
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<Ignore, string> consumeResult = _consumer.Consume(300);
                    
                    if (consumeResult is null)
                        continue;
                    
                    var receivedEventArgs = new ReceivedEventDetails(
                        consumeResult.Topic.Split(".").First(),
                        consumeResult.Topic.Split(".").Last(),
                        consumeResult.Message.Value, consumeResult);
                    
                    OnReceived(receivedEventArgs);
                }
                catch (Exception e)
                {
                    _logger?.LogError("Consume error {Error}. Topic {Topic}", e.Message, topic);
                }
            }
        }, _cancellationToken);
        
        WaitPartitions();
    }

    private void WaitPartitions()
    {
        for (var i = 0; i < 60; i++)
        {
            if (_consumer!.Assignment.Count != 0) {
                break;
            }
            
            Thread.Sleep(500);
        }
    }
    
    protected virtual void OnReceived(ReceivedEventDetails e)
    {
        Received?.Invoke(this, e);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
        _consumer?.Dispose();
        GC.SuppressFinalize(this);
    }
}