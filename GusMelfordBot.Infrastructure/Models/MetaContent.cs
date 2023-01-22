namespace GusMelfordBot.Infrastructure.Models;

public class MetaContent : AuditableEntity
{
    public int? Height { get; set; }
    public int? Width { get; set; }
    public int? Duration { get; set; }
    public int? TelegramMessageId { get; set; }
    public bool IsSaved { get; set; }
}