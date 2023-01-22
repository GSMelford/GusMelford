namespace GusMelfordBot.Infrastructure.Models;

public class Group : AuditableEntity
{
    public Feature Feature { get; set; } = null!;
    public long ChatId { get; set; }
}