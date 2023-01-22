namespace GusMelfordBot.Infrastructure.Models;

public class AttemptMessage : AuditableEntity
{
    public Feature? Feature { get; set; }
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}