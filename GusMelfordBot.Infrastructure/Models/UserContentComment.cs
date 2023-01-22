namespace GusMelfordBot.Infrastructure.Models;

public class UserContentComment : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Message { get; set; } = null!;
}