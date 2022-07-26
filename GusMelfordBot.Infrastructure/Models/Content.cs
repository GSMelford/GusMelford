using System.ComponentModel.DataAnnotations.Schema;

namespace GusMelfordBot.Infrastructure.Models;

public class Content : BaseEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Number { get; set; }
    public User User { get; set; }
    public TelegramChat Chat { get; set; }
    public string? Provider { get; set; }
    public string? OriginalLink { get; set; }
    public string? Path { get; set; }
    public long? MessageId { get; set; }
    public string? AccompanyingCommentary { get; set; }
    public Guid? SameContentId { get; set; }
    public bool? IsValid { get; set; }
    public bool IsViewed { get; set; }
    public bool IsSaved { get; set; }
    public ICollection<TelegramChat> TelegramChats { get; set; } = new List<TelegramChat>();
}