namespace GusMelfordBot.Infrastructure.Models;

public class User : AuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? RefreshToken { get; set; }
    public string? Password { get; set; }
    public Role Role { get; set; } = null!;
    public AuthorizationUserDatum AuthorizationUserDatum { get; set; } = null!;
}