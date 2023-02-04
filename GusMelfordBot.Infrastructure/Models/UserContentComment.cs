namespace GusMelfordBot.Infrastructure.Models;

public class UserContentComment : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ContentId { get; set; }
    public Content Content { get; set; } = null!;
    public string Message { get; set; } = null!;
}