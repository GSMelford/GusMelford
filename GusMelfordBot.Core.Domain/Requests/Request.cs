namespace GusMelfordBot.Core.Domain.Requests;

public class Request
{
    public Request()
    {
        
    }
    
    public Request(
        HttpMethod? httpMethod, 
        string requestUri, 
        Dictionary<string, string>? headers, 
        Dictionary<string, string>? parameters,
        object? body)
    {
        HttpMethod = httpMethod;
        RequestUri = requestUri;
        Headers = headers;
        Parameters = parameters;
        Body = body;
    }

    public HttpMethod? HttpMethod { get; set; }
    public string? RequestUri { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public Dictionary<string, string>? Parameters { get; set; }
    public object? Body { get; set; }
}