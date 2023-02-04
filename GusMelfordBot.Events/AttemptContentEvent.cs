using GusMelfordBot.SimpleKafka.Models;

namespace GusMelfordBot.Events;

public class AttemptContentEvent : BaseEvent
{
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}