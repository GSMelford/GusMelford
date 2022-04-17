namespace GusMelfordBot.Core.Domain.Requests;

public interface IRequestService
{
    Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage httpRequestMessage);
}