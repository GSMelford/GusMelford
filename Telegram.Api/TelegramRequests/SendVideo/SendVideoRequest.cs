using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using Bot.Api.BotRequests;
using Bot.Api.BotRequests.Interfaces;
using Bot.Api.Collection;

namespace Telegram.API.TelegramRequests.SendVideo
{
    public class SendVideoRequest : Request
    {
        protected override string MethodName => "sendVideo";
        protected override HttpMethod Method => HttpMethod.Post;
        
        public SendVideoRequest(string baseUrl, IParameters parameters) 
            : base(baseUrl, parameters)
        {
        }
        
        //TODO Вынести это в отдельный класс и вообще переписать Request, где будет билдиться реквест через контент
        protected override HttpRequestMessage MakeRequest() 
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            ParameterCollection parameters = Parameters?.BuildParameters();
            ParameterCollection headers = Headers?.GetHeaders();

            UriBuilder uriBuilder = new UriBuilder(BaseUrl + MethodName);
            
            if (parameters?.Count > 0)
            {
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                foreach (Parameter parameter in parameters.Cast<Parameter>())
                {
                    object value = parameter.Value;
                    if (value is VideoFile videoFile) //TODO Обобщить до типа Stream
                    {
                        string fileName = videoFile.VideoName ?? "video";
                        string contentDisposition = $@"form-data; name=""{"video"}"";filename=""{fileName}""";
                        
                        contentDisposition = new string(Encoding.UTF8.GetBytes(contentDisposition).Select(Convert.ToChar).ToArray());
                        HttpContent mediaPartContent = new StreamContent(videoFile.VideoStream)
                        {
                            Headers =
                            {
                                {"Content-Type", "application/octet-stream"},
                                {"Content-Disposition", contentDisposition}
                            }
                        };

                        multipartFormDataContent.Add(mediaPartContent, "video", fileName);
                    }
                    else if (value is not null)
                    {
                        multipartFormDataContent.Add(new StringContent(value.ToString()!), parameter.Key);
                    }
                }

                httpRequestMessage.Content = multipartFormDataContent;
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
    }
}