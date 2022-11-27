using System.Text.RegularExpressions;
using ContentCollector.Domain.ContentProviders;
using ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
using RestSharp;

namespace ContentCollector.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class VideoInformationHandler : AbstractTikTokContentHandler
{
    private static readonly Regex RegexGetDownloadLink = new ("\"downloadAddr\":\"(\\S+?)\"", RegexOptions.Compiled);
    private static readonly Regex RegexHeight = new ("\"height\":(\\d*)", RegexOptions.Compiled);
    private static readonly Regex RegexWidth = new ("\"width\":(\\d*)", RegexOptions.Compiled);
    private static readonly Regex RegexDuration = new ("\"duration\":(\\d*)", RegexOptions.Compiled);
    
    public override async Task<ProcessedTikTokContent?> Handle(ProcessedTikTokContent processedTikTokContent)
    {
        RestRequest restRequest = new RestRequest(processedTikTokContent.OriginalLink) { Timeout = 60000 };
        string content = new RestClient().Execute(restRequest).Content!;
        
        if (string.IsNullOrEmpty(content))
        {
            return processedTikTokContent;
        }
        
        processedTikTokContent.VideoStatusCode = 0;
        processedTikTokContent.DownloadLink = Regex.Unescape(RegexGetDownloadLink.Match(content).Groups[1].Value);
        processedTikTokContent.Height = RegexHeight.Match(content).Groups[1].Value.ToInt();
        processedTikTokContent.Width = RegexWidth.Match(content).Groups[1].Value.ToInt();
        processedTikTokContent.Duration = RegexDuration.Match(content).Groups[1].Value.ToInt();
        return await base.Handle(processedTikTokContent);
    }
}