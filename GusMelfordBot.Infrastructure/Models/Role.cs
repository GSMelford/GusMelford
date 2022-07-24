namespace GusMelfordBot.Infrastructure.Models;

public class Role : BaseEntity<Guid>
{
    public string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}