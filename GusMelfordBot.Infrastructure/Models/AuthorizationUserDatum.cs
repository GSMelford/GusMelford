namespace GusMelfordBot.Infrastructure.Models;

public class AuthorizationUserDatum : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string? RefreshToken { get; set; }
}