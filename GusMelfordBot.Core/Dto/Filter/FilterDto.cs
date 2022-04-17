namespace GusMelfordBot.Core.Dto.Filter;

public class FilterDto
{
    public string ChatId { get; set; }
    public string? IsNotViewed { get; set; }
    public string? SinceDateTime { get; set; }
    public string? UntilDateTime { get; set; }
    public string? ContentProviders { get; set; }
}