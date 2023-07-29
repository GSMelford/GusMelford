using Confluent.Kafka;
using Kyoto.Kafka.Interfaces;
using Kyoto.Kafka.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kyoto.Kafka.Services;

public class KafkaConsumerFactory : IKafkaConsumerFactory
{
    private readonly ILogger<IKafkaConsumerFactory>? _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKafkaTopicFactory _kafkaTopicFactory;

    private readonly Dictionary<string, IKafkaConsumerService> _consumers = new();

    public KafkaConsumerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _kafkaTopicFactory = _serviceProvider.GetRequiredService<IKafkaTopicFactory>();
        _logger = _serviceProvider.GetService<ILogger<IKafkaConsumerFactory>>();
    } 
    
    public async Task SubscribeAsync<TEvent, THandler>(
        ConsumerConfig? config = null,
        string? topicPrefix = null,
        string? groupId = null,
        bool? enableAutoCommit = true) where THandler : class, IKafkaHandler<TEvent> where TEvent : BaseEvent
    {
        var eventName = typeof(TEvent).Name;
        string topic = eventName;
        if (!string.IsNullOrEmpty(topicPrefix)) {
            topic = $"{topicPrefix}.{topic}";
        }
        
        string handlerName = typeof(THandler).Name;
        if (string.IsNullOrEmpty(groupId)) {
            groupId = handlerName;
        }

        await _kafkaTopicFactory.CreateTopicIfNotExistAsync(topic, new Dictionary<string, string>{{"bootstrap.servers", config!.BootstrapServers}});
        if (!_consumers.ContainsKey(topic))
        {
            _consumers[topic] = BuildConsumer(topic, groupId, enableAutoCommit, config);
        }
        
        _consumers[topic].Received += KafkaConsumerOnReceived<TEvent, THandler>;
        _logger?.LogInformation("Subscribed to {Event}: {Handler}", eventName, handlerName);
    }

    private IKafkaConsumerService BuildConsumer(
        string topic,
        string groupId, 
        bool? enableAutoCommit,
        ConsumerConfig? config)
    {
        IKafkaConsumerService kafkaConsumerService = new KafkaConsumerService(config, _serviceProvider.GetService<ILogger<IKafkaConsumerService>>());
        kafkaConsumerService.Consume(topic, groupId, enableAutoCommit ?? true);
        _logger?.LogInformation("Consume has been started. Topic: {Topic}", topic);
        return kafkaConsumerService;
    }

    private void KafkaConsumerOnReceived<TEvent, THandler>(object? _, ReceivedEventDetails e) where THandler : class, IKafkaHandler<TEvent> where TEvent : BaseEvent
    {
        var @event = JsonConvert.DeserializeObject<TEvent>(e.Message);
        
        using var scope = _serviceProvider.CreateScope();
        var handler = ActivatorUtilities.CreateInstance(scope.ServiceProvider, typeof(THandler)) as THandler;
        var method = handler?.GetType().GetMethod("HandleAsync");
            
        if (@event != null)
        {
            _logger?.LogInformation("{CommandHandler} started processing. SessionId: {SessionId}",
                typeof(THandler).Name, @event.SessionId);

            try
            {
                if (method!.Invoke(handler, new object[] { @event }) is Task task)
                {
                    task.Wait();
                }
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception,"{CommandHandler} caught an exception. SessionId: {SessionId}",
                    typeof(THandler).Name, @event.SessionId);
            }
                
            _logger?.LogInformation("{CommandHandler} has been processed. SessionId: {SessionId}",
                typeof(THandler).Name, @event.SessionId);
        }
    }
    
    public void Dispose() 
    {
        _consumers.ToList().ForEach(x => x.Value.Dispose());
        GC.SuppressFinalize(this);
    }
}