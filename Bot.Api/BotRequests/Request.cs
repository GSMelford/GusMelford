using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bot.Api.BotRequests.Interfaces;
using Bot.Api.Collection;

namespace Bot.Api.BotRequests
{
    public abstract class Request : IRequest
    {
        protected string BaseUrl { get; }
        protected abstract string MethodName { get; }
        protected abstract HttpMethod Method { get; }

        protected IParameters Parameters { get; }
        protected IHeaders Headers { get; }
        
        protected Request(string baseUrl, IParameters parameters = null, IHeaders headers = null)
        {
            BaseUrl = baseUrl;
            Parameters = parameters;
            Headers = headers;
        }
        
        protected virtual HttpRequestMessage MakeRequest()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            ParameterCollection parameters = Parameters?.BuildParameters();
            ParameterCollection headers = Headers?.GetHeaders();

            UriBuilder uriBuilder = new UriBuilder(BaseUrl + MethodName);
            
            if (parameters?.Count > 0)
            {
                uriBuilder.Query = BuildQuery(parameters);
            }

            if (headers?.Count > 0)
            {
                TryAddHeaderWithoutValidation(httpRequestMessage, headers);
            }

            Uri uri = new Uri(uriBuilder.Uri.AbsoluteUri);
            httpRequestMessage.RequestUri = uri;
            httpRequestMessage.Method = Method;
            return httpRequestMessage;
        }

        protected bool TryAddHeaderWithoutValidation(HttpRequestMessage httpRequestMessage, ParameterCollection headers)
        {
            return headers
                .Cast<Parameter>()
                .All(header => !httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.ToString()));
        }
        
        private string BuildQuery(ParameterCollection parameters)
        {
            List<string> list = new List<string>();
            foreach (Parameter pair in parameters)
            {
                list.Add($"{pair.Key}={pair.Value}");
            }
            return string.Join("&", list);
        }

        public async Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            return await httpClient.SendAsync(MakeRequest());
        }
    }
}