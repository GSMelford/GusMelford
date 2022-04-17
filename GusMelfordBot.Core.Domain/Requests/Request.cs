namespace GusMelfordBot.Core.Domain.Requests;

public class Request
{
    public HttpMethod? HttpMethod { get; set; }
    public string RequestUri { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public object? Body { get; set; }
}