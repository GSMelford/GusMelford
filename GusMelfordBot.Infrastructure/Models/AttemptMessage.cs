namespace GusMelfordBot.Infrastructure.Models;

public class AttemptMessage : AuditableEntity
{
    public Guid FeatureId { get; set; }
    public Feature? Feature { get; set; }
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public string Message { get; set; } = null!;
    public int Attempt { get; set; }
}