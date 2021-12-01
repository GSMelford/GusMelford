namespace GusMelfordBot.Core.Services.Requests
{
    using System.Net.Http.Headers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    
    public class Request
    {
        private readonly HttpMethod _httpMethod;
        private Dictionary<string, string> _headers;
        private Dictionary<string, string> _parameters;
        private object _body;
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

        public Request AddBody(object body)
        {
            _body = body;
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

            if (_body is not null)
            {
                httpRequestMessage.Content = new StringContent(_body.ToString()!);
                httpRequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }
            
            httpRequestMessage.Method = _httpMethod;
            httpRequestMessage.RequestUri = new Uri(_requestUri);
            return httpRequestMessage;
        }
        
        private static string BuildQuery(Dictionary<string, string> parameters)
        {
            return "?" + string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"));
        }
    }
}