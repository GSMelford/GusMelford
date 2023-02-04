namespace GusMelfordBot.Infrastructure.Models;

public class FunnyPhrase : AuditableEntity
{
    public string Text { get; set; } = null!;
    public Guid? UserId { get; set; }
    public User? User { get; set; }
}