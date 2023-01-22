namespace GusMelfordBot.Infrastructure.Models;

public class Content : AuditableEntity
{
    public string Provider { get; set; } = null!;
    public string OriginalLink { get; set; } = null!;
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public MetaContent MetaContent { get; set; } = null!;
    public ICollection<UserContentComment> UserContentComments { get; set; } = new List<UserContentComment>();
    public ICollection<User> Users { get; set; } = new List<User>();
}