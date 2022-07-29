using System.Text.RegularExpressions;
using ContentCollector.Domain.ContentProviders;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok;

public static class TikTokServiceExtension
{
    public static ProcessedTikTokContent? ToBasicProcessedContent(Guid contentId, string messageText)
    {
        string? sentTikTokLink = new Regex(@"https://\w*.tiktok.com/\S*")
            .Matches(messageText)
            .FirstOrDefault()?.Value;

        if (string.IsNullOrEmpty(sentTikTokLink)) {
            return null;
        }
        
        return new ProcessedTikTokContent
        {
            ContentId = contentId,
            AccompanyingCommentary = messageText.Replace(sentTikTokLink, ""),
            OriginalLink = sentTikTokLink,
            Provider = nameof(ContentProvider.TikTok)
        };
    }
    
    public static async Task<string?> GetRefererLink(this string sentLink)
    {
        Uri? uri = (await new RestClient().ExecuteAsync(
            new RestRequest(sentLink) { Timeout = 60000 })).ResponseUri;
        
        if (uri is null) {
            return null;
        }
        
        return uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
    }

    public static string BuildVideoInformationUrl(this string refererLink)
    {
        UserVideo userVideo = GetUserVideo(refererLink);
        return $"https://www.tiktok.com/node/share/video/{userVideo.Username}/{userVideo.VideoId}";
    }

    public static string BuildPathToContent(this string refererLink)
    {
        UserVideo userVideo = GetUserVideo(refererLink);
        return Path.Combine(Constants.ContentFolder, $"{userVideo.Username}-{userVideo.VideoId}.mp4");
    }

    private static UserVideo GetUserVideo(string refererLink)
    {
        UserVideo userVideo = new UserVideo();
        Match match = Regex.Match(refererLink, "com/(.*?)/video/(.*)");

        userVideo.Username = match.Groups[1].Value;
        userVideo.VideoId = match.Groups[2].Value;

        return userVideo;
    }
    
    public static ContentProcessedEvent ToContentProcessedEvent(this ProcessedTikTokContent processedTikTokContent)
    {
        return new ContentProcessedEvent
        {
            Id = processedTikTokContent.ContentId,
            Path = processedTikTokContent.Path,
            Provider = processedTikTokContent.Provider,
            AccompanyingCommentary = processedTikTokContent.AccompanyingCommentary,
            IsValid = processedTikTokContent.IsValid,
            OriginalLink = processedTikTokContent.OriginalLink,
            IsSaved = processedTikTokContent.IsSaved,
            Height = processedTikTokContent.Height,
            Width = processedTikTokContent.Width,
            Duration = processedTikTokContent.Duration
        };
    }
}