using System.Text.RegularExpressions;
using ContentCollector.KafkaEventHandlers.Events;
using ContentCollector.MircoService.Domain.ContentProviders;
using ContentCollector.MircoService.Domain.ContentProviders.TikTok;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok;

public static class TikTokServiceExtension
{
    public static ProcessedContent? ToBasicProcessedContent(Guid contentId, string messageText)
    {
        string? sentTikTokLink = new Regex(@"https://\w*.tiktok.com/\S*")
            .Matches(messageText)
            .FirstOrDefault()?.Value;

        if (string.IsNullOrEmpty(sentTikTokLink)) {
            return null;
        }
        
        string accompanyingCommentary = messageText.Replace(sentTikTokLink, "");
        return new ProcessedContent
        {
            ContentId = contentId,
            AccompanyingCommentary = accompanyingCommentary,
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
        return Path.Combine(Constants.CONTENT_FOLDER, $"{userVideo.Username}-{userVideo.VideoId}");
    }

    private static UserVideo GetUserVideo(string refererLink)
    {
        UserVideo userVideo = new UserVideo();
        var a = Regex.Match(refererLink, "com/(.*?)/video/(.*)");

        userVideo.Username = a.Groups[1].Value;
        userVideo.VideoId = a.Groups[2].Value;

        return userVideo;
    }
    
    public static ContentProcessedEvent ToContentProcessedEvent(this ProcessedContent processedContent)
    {
        return new ContentProcessedEvent
        {
            Id = processedContent.ContentId,
            Path = processedContent.Path,
            Provider = processedContent.Provider,
            AccompanyingCommentary = processedContent.AccompanyingCommentary,
            IsValid = processedContent.IsValid,
            OriginalLink = processedContent.OriginalLink,
            IsSaved = processedContent.IsSaved
        };
    }
}