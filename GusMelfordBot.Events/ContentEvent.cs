using GusMelfordBot.SimpleKafka.Events;

namespace GusMelfordBot.Events;

public class ContentEvent : BaseEvent
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}