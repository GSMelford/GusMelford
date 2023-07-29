using System.Linq;
using System.Text.RegularExpressions;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using GusMelfordBot.Events;

namespace ContentProcessor.Worker.Domain;

public static class Converter
{
    public static ProcessTikTokContent ToDomain(this ContentEvent contentEvent)
    {
        string messageText = contentEvent.Message;
        string originalLink = new Regex(@"https://\w*.tiktok.com/\S*")
            .Matches(messageText)
            .FirstOrDefault()?.Value ?? string.Empty;
        
        return new ProcessTikTokContent
        {
            Id = contentEvent.SessionId,
            UserComment = messageText.Replace(originalLink, "").Trim(),
            OriginalLink = originalLink,
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
            SessionId = processTikTokContent.Id,
            Provider = processTikTokContent.Provider,
            UserComment = processTikTokContent.UserComment,
            OriginalLink = processTikTokContent.OriginalLink,
            IsSaved = processTikTokContent.IsSaved,
            Height = processTikTokContent.Height,
            Width = processTikTokContent.Width,
            Duration = processTikTokContent.Duration,
            UserId = processTikTokContent.UserId,
            GroupId = processTikTokContent.GroupId
        };
    }
    
    public static AttemptContentEvent ToAttemptContentEvent(this ProcessTikTokContent processTikTokContent)
    {
        return new AttemptContentEvent
        {
            SessionId = processTikTokContent.Id,
            Message = $"{processTikTokContent.OriginalLink} {processTikTokContent.UserComment}",
            Attempt = processTikTokContent.Attempt,
            UserId = processTikTokContent.UserId,
            GroupId = processTikTokContent.GroupId
        };
    }
}