using Confluent.Kafka;

namespace Kyoto.Kafka.Modules;

public class ReceivedEventDetails
{ 
    public string TenantKey { get; }
    public string Topic { get; }
    public string Message { get; }
    public ConsumeResult<Ignore, string> ConsumeResult { get; }
    
    public ReceivedEventDetails(string tenantKey, string topic, string message, ConsumeResult<Ignore, string> consumeResult)
    {
        TenantKey = tenantKey;
        Topic = topic;
        Message = message;
        ConsumeResult = consumeResult;
    }
}