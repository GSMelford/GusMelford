using System.Text.RegularExpressions;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.KafkaEventHandlers.Events;

namespace ContentProcessor.Worker.Domain;

public static class Converter
{
    public static ProcessTikTokContent ToDomain(this ContentEvent contentEvent)
    {
        string messageText = contentEvent.Message;
        return new ProcessTikTokContent
        {
            Id = contentEvent.Id,
            UserComment = messageText.Replace(messageText, "").Trim(),
            OriginalLink = new Regex(@"https://\w*.tiktok.com/\S*")
                .Matches(messageText)
                .FirstOrDefault()?.Value ?? string.Empty,
            Provider = nameof(ContentProvider.TikTok),
            Attempt = contentEvent.Attempt,
            UserId = contentEvent.UserId,
            GroupId = contentEvent.GroupId
        };
    }

    public static ContentProcessedEvent ToContentProcessedEvent(this ProcessTikTokContent processTikTokContent)
    {
        return new ContentProcessedEvent
        {
            Id = processTikTokContent.Id,
            Provider = processTikTokContent.Provider,
            UserComment = processTikTokContent.UserComment,
            OriginalLink = processTikTokContent.OriginalLink,
            IsSaved = processTikTokContent.IsSaved,
            Height = processTikTokContent.Height,
            Width = processTikTokContent.Width,
            Duration = processTikTokContent.Duration
        };
    }
    
    public static AttemptContentEvent ToAttemptContentEvent(this ProcessTikTokContent processTikTokContent)
    {
        return new AttemptContentEvent
        {
            Id = processTikTokContent.Id,
            Message = processTikTokContent.OriginalLink,
            Attempt = processTikTokContent.Attempt,
            UserId = processTikTokContent.UserId,
            GroupId = processTikTokContent.GroupId
        };
    }
}