using GusMelfordBot.Core.Domain.Requests;

namespace GusMelfordBot.Core.Services.Requests;
    
public class RequestService : IRequestService
{
    private readonly HttpClient _httpClient;

    public RequestService()
    {
        _httpClient = new HttpClient();
    }
        
    public async Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage httpRequestMessage)
    {
        return await _httpClient.SendAsync(httpRequestMessage);
    }
}