namespace GusMelfordBot.Infrastructure.Models;

public class Role : AuditableEntity
{
    public string Name { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}