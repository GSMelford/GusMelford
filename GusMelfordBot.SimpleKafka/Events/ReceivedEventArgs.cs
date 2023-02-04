using Confluent.Kafka;

namespace GusMelfordBot.SimpleKafka.Models;

public class ReceivedEventArgs
{ 
    public string Topic { get; }
    public string Message { get; }
    public ConsumeResult<Ignore, string> ConsumeResult { get; }
    
    public ReceivedEventArgs(string topic, string message, ConsumeResult<Ignore, string> consumeResult)
    {
        Topic = topic;
        Message = message;
        ConsumeResult = consumeResult;
    }
}