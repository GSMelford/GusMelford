namespace GusMelfordBot.Infrastructure.Models;

public class User : BaseEntity
{
    public Role? Role { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public ICollection<Content> Contents { get; set; } = new List<Content>();
}