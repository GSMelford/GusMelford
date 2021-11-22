namespace GusMelfordBot.Core.Services
{
    using Telegram.API.TelegramRequests.DeleteMessage;
    using Telegram.API.TelegramRequests.SendMessage;
    using Telegram.Dto;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using DAL.TikTok;
    using GusMelfordBot.Database.Interfaces;
    using Interfaces;
    using System.Linq;
    using Telegram.Dto.UpdateModule;
    
    public class TikTokService : ITikTokService
    {
        private readonly IDatabaseContext _databaseContext;
        private readonly List<DAL.User> _users;
        private readonly HttpClient _httpClient;
        private readonly IGusMelfordBotService _gusMelfordBotService;
        private int _videoCounter;
        
        public TikTokService(
            IDatabaseContext context, 
            IGusMelfordBotService gusMelfordBotService)
        {
            _httpClient = new HttpClient();
            _databaseContext = context;
            _videoCounter = _databaseContext.Count<DAL.TikTok.Video>().Result;
            _users = _databaseContext.Get<DAL.User>();
            _gusMelfordBotService = gusMelfordBotService;
            _gusMelfordBotService.OnMessageUpdate += ProcessMessage;
        }

        private void ProcessMessage(Message message)
        {
            if (!VerifyTikTokMessage(message))
            {
                return;
            }

            AddUserIfNotExist(message.From);
            SaveLink(message);
        }

        private void AddUserIfNotExist(User user)
        {
            if (_users.FirstOrDefault(x => x.TelegramUserId == user.Id) is not null)
            {
                return;
            }
            
            _users.Add(new DAL.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Username,
                TelegramUserId = user.Id
            });
            
            _databaseContext.SaveAll();
        }

        private void SaveLink(Message message)
        {
            string text = message.Text;
            string videoLink = text.Split(' ').FirstOrDefault(l =>
                l.Contains(Constants.TikTokDomain) || l.Contains(Constants.TikTokMobileDomain))?.Trim();

            if (string.IsNullOrEmpty(videoLink))
            {
                return;
            }
            
            string comment = text.Replace(videoLink, "");
            Uri uri = DoRequest(videoLink).RequestMessage?.RequestUri;
            string referer = string.Empty;
            
            if (uri is not null)
            {
                referer = uri.Scheme + "://" + uri.Host + uri.AbsolutePath;
            }

            var user = _users.FirstOrDefault(u => u.TelegramUserId == message.From.Id);
            
            var video = new DAL.TikTok.Video {
                User = user,
                SentLink = videoLink,
                RefererLink = referer
            };
            
            _databaseContext.Add(video);

            if (!string.IsNullOrEmpty(comment))
            {
                _databaseContext.Add(new Comment
                {
                    Text = comment,
                    Video = video
                });
            }
            
            _databaseContext.SaveAll();

            SendMessage(message.Chat, $"{user?.FirstName} sent {++_videoCounter} : {videoLink}");
            DeleteMessage(message);
        }

        private void DeleteMessage(Message message)
        {
            var httpResponseMessage = _gusMelfordBotService.DeleteMessage(
                new DeleteMessageParameters
                {
                    ChatId = message.Chat.Id,
                    MessageId = message.MessageId
                }).Result;

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                /*_logger.LogWarning("{DeleteMessage} - {StatusCode}", 
                    nameof(DeleteMessage), httpResponseMessage.StatusCode);*/
            }
        }

        private void SendMessage(Chat chat, string message)
        {
            _gusMelfordBotService.SendMessage(new SendMessageParameters
            {
                Text = message,
                ChatId = chat.Id,
                DisableNotification = true,
                DisableWebPagePreview = true
            });
        }
        
        private HttpResponseMessage DoRequest(string requestUrl)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            httpRequestMessage.Headers.Add("User-Agent", Constants.UserAgent);
            return _httpClient.SendAsync(httpRequestMessage).Result;
        }
        
        private static bool VerifyTikTokMessage(Message message)
        {
            return Constants.TikTokDomains.FirstOrDefault(x => message.Text.Contains(x)) is not null;
        }
    }
}