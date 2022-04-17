namespace GusMelfordBot.Core.Domain.Apps.ContentCollector.Content;

public class Filter
{
    public Guid ChatId { get; set; }
    public bool IsNotViewed { get; set; }
    public DateTime? SinceDateTime { get; set; }
    public DateTime? UntilDateTime { get; set; }
    public List<string>? ContentProviders { get; set; }
}