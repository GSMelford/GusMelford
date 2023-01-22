namespace GusMelfordBot.Domain.Application.ContentCollector;

public class MetaContent
{
    public bool IsSaved { get; }
    public int? Height { get; }
    public int? Width { get; }
    public int? Duration { get; }
    public int? TelegramMessageId { get; }

    public MetaContent(bool isSaved, int? height, int? width, int? duration, int? telegramMessageId = null)
    {
        IsSaved = isSaved;
        Height = height;
        Width = width;
        Duration = duration;
        TelegramMessageId = telegramMessageId;
    }
}