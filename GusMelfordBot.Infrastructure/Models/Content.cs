using System.ComponentModel.DataAnnotations.Schema;

namespace GusMelfordBot.Infrastructure.Models;

public class Content : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Number { get; set; }
    public TelegramChat Chat { get; set; }
    public string? Provider { get; set; }
    public string? OriginalLink { get; set; }
    public string? Path { get; set; }
    public long? MessageId { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
    public bool? IsValid { get; set; }
    public bool IsViewed { get; set; }
    public bool IsSaved { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}