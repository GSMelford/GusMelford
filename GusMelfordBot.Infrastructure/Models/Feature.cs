namespace GusMelfordBot.Infrastructure.Models;

public class Feature : AuditableEntity
{
    public string Name { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}