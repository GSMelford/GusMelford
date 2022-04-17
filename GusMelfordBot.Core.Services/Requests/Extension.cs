using System.Net.Http.Headers;
using GusMelfordBot.Core.Domain.Requests;
using GusMelfordBot.Core.Extensions;

namespace GusMelfordBot.Core.Services.Requests;

public static class Extension
{
    public static HttpRequestMessage ToHttpRequestMessage(this Request request)
    {
        HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            
        if (request.Parameters?.Count > 0)
        {
            request.RequestUri += request.Parameters.BuildQuery();
        }
            
        if (request.Headers?.Count > 0)
        {
            foreach (var (key, value) in request.Headers)
            {
                httpRequestMessage.Headers.Add(key, value);
            }
        }

        if (request.Body is not null)
        {
            httpRequestMessage.Content = new StringContent(request.Body.ToString()!);
            httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
            
        httpRequestMessage.Method = request.HttpMethod ?? HttpMethod.Get;
        if (request.RequestUri != null) httpRequestMessage.RequestUri = new Uri(request.RequestUri);
        return httpRequestMessage;
    }
}