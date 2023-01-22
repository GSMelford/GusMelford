using GusMelfordBot.SimpleKafka.Models;

namespace GusMelfordBot.Events;

public class ContentEvent : BaseEvent
{
    public string GroupId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}