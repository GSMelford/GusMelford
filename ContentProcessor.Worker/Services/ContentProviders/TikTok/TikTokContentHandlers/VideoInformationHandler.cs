using System.Text.RegularExpressions;
using ContentProcessor.Worker.Domain.ContentProviders.TikTok;
using ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers.Abstractions;
using GusMelfordBot.Extensions;
using RestSharp;

namespace ContentProcessor.Worker.Services.ContentProviders.TikTok.TikTokContentHandlers;

public class VideoInformationHandler : AbstractTikTokContentHandler
{
    private static readonly Regex RegexGetDownloadLink = new ("\"downloadAddr\":\"(\\S+?)\"", RegexOptions.Compiled);
    private static readonly Regex RegexHeight = new ("\"height\":(\\d*)", RegexOptions.Compiled);
    private static readonly Regex RegexWidth = new ("\"width\":(\\d*)", RegexOptions.Compiled);
    private static readonly Regex RegexDuration = new ("\"duration\":(\\d*)", RegexOptions.Compiled);
    
    public override async Task<ProcessTikTokContent?> HandleAsync(ProcessTikTokContent processTikTokContent)
    {
        RestRequest restRequest = new RestRequest(processTikTokContent.OriginalLink) { Timeout = 60000 };
        string? content = (await new RestClient().ExecuteAsync(restRequest)).Content;
        
        if (string.IsNullOrEmpty(content)) {
            return processTikTokContent.MarkAsProcessFailed();
        }
        
        processTikTokContent.DownloadLink = Regex.Unescape(RegexGetDownloadLink.Match(content).Groups[1].Value);
        processTikTokContent.Height = RegexHeight.Match(content).Groups[1].Value.ToInt();
        processTikTokContent.Width = RegexWidth.Match(content).Groups[1].Value.ToInt();
        processTikTokContent.Duration = RegexDuration.Match(content).Groups[1].Value.ToInt();
        
        return await base.HandleAsync(processTikTokContent);
    }
}