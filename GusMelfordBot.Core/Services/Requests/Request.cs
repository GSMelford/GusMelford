namespace GusMelfordBot.Core.Services.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    
    public class Request
    {
        private readonly HttpMethod _httpMethod;
        private Dictionary<string, string> _headers;
        private Dictionary<string, string> _parameters;
        private string _requestUri;
        
        public Request(string requestUri, HttpMethod httpMethod = null)
        {
            _requestUri = requestUri;
            _httpMethod = httpMethod ?? HttpMethod.Get;
        }

        public Request AddHeaders(Dictionary<string, string> headers)
        {
            _headers = headers;
            return this;
        }
        
        public Request AddParameters(Dictionary<string, string> parameters)
        {
            _parameters = parameters;
            return this;
        }

        public HttpRequestMessage Build()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            
            if (_parameters?.Count > 0)
            {
                _requestUri += BuildQuery(_parameters);
            }
            
            if (_headers?.Count > 0)
            {
                foreach (var (key, value) in _headers)
                {
                    httpRequestMessage.Headers.Add(key, value);
                }
            }
            
            httpRequestMessage.Method = _httpMethod;
            httpRequestMessage.RequestUri = new Uri(_requestUri);
            return httpRequestMessage;
        }
        
        private string BuildQuery(Dictionary<string, string> parameters)
        {
            return "?" + string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        }
    }
}