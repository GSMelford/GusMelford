namespace GusMelfordBot.Core.Interfaces
{
    using System.Net.Http;
    using System.Threading.Tasks;
    
    public interface IRequestService
    {
        Task<HttpResponseMessage> ExecuteAsync(HttpRequestMessage httpRequestMessage);
    }
}